namespace University.IntegrationTests;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Infrastructure.Data;
using University.Web.Services;

public class ThesisArtifactStorageServiceTests
{
    [Fact]
    public async Task GetOwnedByArtifactIdAsync_ReturnsArtifact_ForOwnerOnly()
    {
        await using var db = CreateDbContext(nameof(GetOwnedByArtifactIdAsync_ReturnsArtifact_ForOwnerOnly));

        var ownerUser = new IdentityUser
        {
            Id = "student-owner",
            UserName = "student1@univ.edu",
            NormalizedUserName = "STUDENT1@UNIV.EDU",
            Email = "student1@univ.edu",
            NormalizedEmail = "STUDENT1@UNIV.EDU",
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };

        var otherUser = new IdentityUser
        {
            Id = "student-other",
            UserName = "student2@univ.edu",
            NormalizedUserName = "STUDENT2@UNIV.EDU",
            Email = "student2@univ.edu",
            NormalizedEmail = "STUDENT2@UNIV.EDU",
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };

        db.Users.AddRange(ownerUser, otherUser);

        var ownerStudent = Student.Create(ownerUser.Id, "AI");
        var otherStudent = Student.Create(otherUser.Id, "AI");

        db.Students.AddRange(ownerStudent, otherStudent);
        await db.SaveChangesAsync();

        var artifact = ThesisArtifact.Create(
            thesisId: Guid.NewGuid(),
            studentId: ownerStudent.Id,
            fileName: "artifact.zip",
            contentType: "application/zip",
            data: new byte[] { 1, 2, 3, 4 });

        db.ThesisArtifacts.Add(artifact);
        await db.SaveChangesAsync();

        var sut = new ThesisArtifactStorageService(db);

        var owned = await sut.GetOwnedByArtifactIdAsync(ownerUser.Id, artifact.ArtifactId);
        var notOwned = await sut.GetOwnedByArtifactIdAsync(otherUser.Id, artifact.ArtifactId);

        Assert.NotNull(owned);
        Assert.Equal("artifact.zip", owned!.FileName);
        Assert.Null(notOwned);
    }

    private static UniversityDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new UniversityDbContext(options);
    }
}
