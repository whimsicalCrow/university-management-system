using University.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<UserSessionService>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<University.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
