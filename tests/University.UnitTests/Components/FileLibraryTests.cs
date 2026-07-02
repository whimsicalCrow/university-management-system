namespace University.UnitTests.Components;

using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using University.Web.Components.Pages;
using University.Web.Services;

public class FileLibraryTests : TestContext
{
    [Fact]
    public void FileLibrary_DeclaresExpectedRoute()
    {
        var route = typeof(FileLibrary)
            .GetCustomAttributes(inherit: false)
            .OfType<Microsoft.AspNetCore.Components.RouteAttribute>()
            .SingleOrDefault();

        Assert.NotNull(route);
        Assert.Equal("/file-library", route!.Template);
    }

    [Fact]
    public void FileLibrary_WhenUserNotAuthenticated_ShowsAccessMessage()
    {
        Services.AddScoped<UserSessionService>();
        Services.AddSingleton<IStudentDashboardService>(
            new StubDashboardService(StudentDashboardData.Empty));

        var cut = RenderComponent<FileLibrary>();

        Assert.Contains("Please sign in as a student to use the file library.", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void FileLibrary_WhenAuthenticatedStudent_RendersPersistedFiles()
    {
        Services.AddScoped<UserSessionService>();

        var data = new StudentDashboardData(
            StudentId: 11,
            ActiveTheses:
            [
                new StudentThesisCard(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Adaptive AI Planning", "Ongoing", DateTime.UtcNow.Date.AddDays(5), "prof1@univ.edu")
            ],
            RecentComments:
            [
                new DashboardCommentItem(12, "prof1@univ.edu", "Looks good", DateTime.UtcNow)
            ],
            ActionItems:
            [
                new DashboardActionItem("Publish update", "Share your next milestone progress.", DateTime.UtcNow.Date.AddDays(2), "/updates")
            ],
            FileLibraryRecords:
            [
                new DashboardFileRecord(
                    Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    "literature-review-pack.zip",
                    2_048_000,
                    DateTime.UtcNow.Date,
                    "/api/thesis-artifacts/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
            ]);

        Services.AddSingleton<IStudentDashboardService>(new StubDashboardService(data));

        var session = Services.GetRequiredService<UserSessionService>();
        session.Login("student1@univ.edu", UserRole.Student);

        var cut = RenderComponent<FileLibrary>();

        Assert.Contains("Thesis File Library", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("literature-review-pack.zip", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Download artifact", cut.Markup, StringComparison.Ordinal);
    }

    private sealed class StubDashboardService : IStudentDashboardService
    {
        private readonly StudentDashboardData _data;

        public StubDashboardService(StudentDashboardData data)
        {
            _data = data;
        }

        public Task<StudentDashboardData> GetDashboardAsync(string? userName, CancellationToken cancellationToken = default)
            => Task.FromResult(_data);
    }
}