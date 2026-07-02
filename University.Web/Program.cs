using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MediatR;
using University.Application.Commands;
using University.Infrastructure.Data;
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

builder.Services.AddScoped<UserSessionService>();
builder.Services.AddSingleton<IThesisInterestService, ThesisInterestService>();
builder.Services.AddScoped<IThesisTopicAssignmentService, ThesisTopicAssignmentService>();

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
