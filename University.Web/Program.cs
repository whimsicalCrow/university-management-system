using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MediatR;
using University.Application.Commands;
using University.Application.Interfaces;
using University.Infrastructure.Data;
using University.Infrastructure.Repositories;
using University.Infrastructure.Services;
using University.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration.GetConnectionString("UniversityDatabase")
    ?? throw new InvalidOperationException(
        "Missing SQL connection string. Configure 'ConnectionStrings:DefaultConnection' or 'ConnectionStrings:UniversityDatabase'.");

// Add Entity Framework Core with SQL Server
builder.Services.AddDbContext<UniversityDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    // Suppress non-deterministic model warnings (seed data is static)
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add ASP.NET Core Identity
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<UniversityDbContext>()
    .AddDefaultTokenProviders();

// Add Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("StudentOnly", policy => policy.RequireRole("Student"))
    .AddPolicy("ProfessorOnly", policy => policy.RequireRole("Professor"));

// Add MediatR
builder.Services.AddMediatR(typeof(University.Application.Commands.LoginCommand));

// Add Data Protection (used for signed download tokens)
builder.Services.AddDataProtection();

// Register attachment storage provider (Local or AzureBlob based on configuration)
var storageProvider = builder.Configuration["Attachments:StorageProvider"] ?? "Local";
if (storageProvider.Equals("AzureBlob", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IAttachmentStorageService, AzureBlobAttachmentStorageService>();
}
else
{
    var localPath = Path.Combine(
        builder.Environment.WebRootPath,
        builder.Configuration["Attachments:LocalStoragePath"] ?? "attachments");
    builder.Services.AddScoped<IAttachmentStorageService>(_ =>
        new LocalFileAttachmentStorageService(localPath));
}

// Register virus scan service (NoOp placeholder; swap for real implementation when available)
builder.Services.AddScoped<IVirusScanService, NoOpVirusScanService>();

// Register artifact repository
builder.Services.AddScoped<IThesisArtifactRepository, ThesisArtifactRepository>();

// Register thesis-update repository (used by SubmitReviewCommandHandler)
builder.Services.AddScoped<University.Application.Interfaces.IThesisUpdateRepository,
    University.Infrastructure.Repositories.ThesisUpdateRepository>();

builder.Services.AddScoped<UserSessionService>();
builder.Services.AddSingleton<IThesisInterestService, ThesisInterestService>();
builder.Services.AddScoped<IThesisTopicAssignmentService, ThesisTopicAssignmentService>();
builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();
builder.Services.AddScoped<University.Web.Services.IThesisTimelineService,
    University.Web.Services.ThesisTimelineService>();

// Register full-pipeline ThesisArtifactStorageService
builder.Services.AddScoped<IThesisArtifactStorageService>(sp => new ThesisArtifactStorageService(
    sp.GetRequiredService<UniversityDbContext>(),
    sp.GetRequiredService<ISender>(),
    sp.GetRequiredService<IAttachmentStorageService>(),
    sp.GetRequiredService<Microsoft.AspNetCore.DataProtection.IDataProtectionProvider>(),
    sp.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>()));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    db.Database.Migrate();
    await EnsureDemoUserPasswordsAsync(userManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Add authentication & authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<University.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/auth/login",
    async (LoginApiRequest request, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager) =>
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.BadRequest(new LoginApiResponse(false, null, "Email and password are required."));
        }

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var signInResult = await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            return Results.Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Unknown";

        return Results.Ok(new LoginApiResponse(true, role, null));
    })
    .DisableAntiforgery();

app.MapGet("/auth/logout",
    async (SignInManager<IdentityUser> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return Results.Redirect("/login");
    });

app.MapGet("/api/thesis-artifacts/{artifactId:guid}",
    async (Guid artifactId, ClaimsPrincipal user, IThesisArtifactStorageService artifactStorage, CancellationToken cancellationToken) =>
    {
        var identityUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            return Results.Unauthorized();
        }

        var artifact = await artifactStorage.GetOwnedByArtifactIdAsync(identityUserId, artifactId, cancellationToken);
        if (artifact is null)
        {
            return Results.NotFound();
        }

        if (artifact.Data is null)
        {
            // Storage-key-based artifact: use the signed download endpoint instead
            return Results.NotFound();
        }

        return Results.File(
            fileContents: artifact.Data,
            contentType: artifact.ContentType,
            fileDownloadName: artifact.FileName);
    })
    .RequireAuthorization("StudentOnly");

// Signed time-limited download endpoint (new storage-pipeline artifacts)
app.MapGet("/attachments/download/{token}",
    async (
        string token,
        IThesisArtifactStorageService artifactStorage,
        ClaimsPrincipal user,
        ILogger<Program> logger,
        CancellationToken cancellationToken) =>
    {
        var identityUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

        var download = await artifactStorage.DownloadByTokenAsync(token, cancellationToken);
        if (download is null)
        {
            logger.LogWarning(
                "Download rejected: invalid or expired token for user {UserId}, token prefix: {TokenPrefix}",
                identityUserId,
                token.Length > 8 ? token[..8] : token);
            return Results.Forbid();
        }

        logger.LogInformation(
            "Artifact download served for user {UserId}, file: {FileName}",
            identityUserId,
            download.FileName);

        return Results.File(download.Stream, download.ContentType, download.FileName);
    })
    .RequireAuthorization();

app.Run();

static async Task EnsureDemoUserPasswordsAsync(UserManager<IdentityUser> userManager)
{
    const string demoPassword = "TempPass123!";

    for (var i = 1; i <= 5; i++)
    {
        var email = $"prof{i}@univ.edu";
        await EnsurePasswordHashAsync(userManager, email, demoPassword);
    }

    for (var i = 1; i <= 15; i++)
    {
        var email = $"student{i}@univ.edu";
        await EnsurePasswordHashAsync(userManager, email, demoPassword);
    }
}

static async Task EnsurePasswordHashAsync(UserManager<IdentityUser> userManager, string email, string password)
{
    var user = await userManager.FindByEmailAsync(email);
    if (user is null)
    {
        return;
    }

    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);
    await userManager.UpdateAsync(user);
}

sealed record LoginApiRequest(string Email, string Password);
sealed record LoginApiResponse(bool Success, string? Role, string? Message);
