using University.Application.DependencyInjection;
using University.Application.Meetings.Notifications;
using University.Infrastructure.DependencyInjection;
using University.Web.Components;
using University.Web.Hubs;
using University.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddApplicationLayer()
    .AddInfrastructureLayer(builder.Configuration);

builder.Services.AddSignalR();
builder.Services.AddScoped<IMeetingActionItemBroadcaster, SignalRMeetingActionItemBroadcaster>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<MeetingActionItemHub>("/hubs/meeting-action-items");

app.Run();
