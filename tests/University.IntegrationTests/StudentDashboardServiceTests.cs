namespace University.IntegrationTests;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Infrastructure.Data;
using University.Web.Services;

public class StudentDashboardServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_WithMappedStudent_ReturnsProjectedData()
    {
        await using var db = CreateDbContext(nameof(GetDashboardAsync_WithMappedStudent_ReturnsProjectedData));

        var studentUser = new IdentityUser
        {
            Id = "student-user-1",
            UserName = "student1@univ.edu",
            NormalizedUserName = "STUDENT1@UNIV.EDU",
            Email = "student1@univ.edu",
            NormalizedEmail = "STUDENT1@UNIV.EDU",
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };

        var professorUser = new IdentityUser
        {
            Id = "prof-user-1",
            UserName = "prof1@univ.edu",
            NormalizedUserName = "PROF1@UNIV.EDU",
            Email = "prof1@univ.edu",
            NormalizedEmail = "PROF1@UNIV.EDU",
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };

        db.Users.Add(studentUser);
        db.Users.Add(professorUser);

        var professor = Professor.Create(professorUser.Id, "Computer Science", "Distributed Systems");
        var student = Student.Create(studentUser.Id, "AI");

        db.Professors.Add(professor);
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var thesis = ThesisTopicAssignment.Create(Guid.NewGuid(), "Adaptive AI Planning", student.Id, professor.Id);
        var update = ThesisUpdate.Create(student.Id, "Completed literature review and baseline architecture.");
        update.Submit();

        db.ThesisTopicAssignments.Add(thesis);
        db.ThesisUpdates.Add(update);
        await db.SaveChangesAsync();

        var feedback = Feedback.Create(update.Id, professor.Id, "Good momentum. Prioritize evaluation metrics next.");
        db.Feedback.Add(feedback);

        var artifact = ThesisArtifact.Create(
            thesis.TopicId,
            student.Id,
            "literature-review-pack.zip",
            "application/zip",
            new byte[] { 0x01, 0x02, 0x03 });
        db.ThesisArtifacts.Add(artifact);

        await db.SaveChangesAsync();

        var sut = new StudentDashboardService(db);

        var result = await sut.GetDashboardAsync(studentUser.Email);

        Assert.NotNull(result.StudentId);
        Assert.Single(result.ActiveTheses);
        Assert.Single(result.RecentComments);
        Assert.NotEmpty(result.ActionItems);
        Assert.Single(result.FileLibraryRecords);

        Assert.Equal("Adaptive AI Planning", result.ActiveTheses[0].TopicTitle);
        Assert.Contains("prof1@univ.edu", result.RecentComments[0].Author, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("literature-review-pack.zip", result.FileLibraryRecords[0].FileName);
        Assert.Equal($"/api/thesis-artifacts/{artifact.ArtifactId}", result.FileLibraryRecords[0].DownloadHref);
    }

    [Fact]
    public async Task GetDashboardAsync_WhenUserHasNoStudentProfile_ReturnsEmpty()
    {
        await using var db = CreateDbContext(nameof(GetDashboardAsync_WhenUserHasNoStudentProfile_ReturnsEmpty));

        db.Users.Add(new IdentityUser
        {
            Id = "prof-user-2",
            UserName = "prof2@univ.edu",
            NormalizedUserName = "PROF2@UNIV.EDU",
            Email = "prof2@univ.edu",
            NormalizedEmail = "PROF2@UNIV.EDU",
            SecurityStamp = Guid.NewGuid().ToString("N"),
        });
        await db.SaveChangesAsync();

        var sut = new StudentDashboardService(db);
        var result = await sut.GetDashboardAsync("prof2@univ.edu");

        Assert.Null(result.StudentId);
        Assert.Empty(result.ActiveTheses);
        Assert.Empty(result.RecentComments);
        Assert.Empty(result.ActionItems);
        Assert.Empty(result.FileLibraryRecords);
    }

    private static UniversityDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new UniversityDbContext(options);
    }
}