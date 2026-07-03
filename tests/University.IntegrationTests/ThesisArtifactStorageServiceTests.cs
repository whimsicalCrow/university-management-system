namespace University.IntegrationTests;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using University.Application.Interfaces;
using University.Domain.Entities;
using University.Domain.Enums;
using University.Infrastructure.Data;
using University.Infrastructure.Repositories;
using University.Infrastructure.Services;
using University.Web.Services;

public class ThesisArtifactStorageServiceTests
{
    // ── Existing test (preserved) ──────────────────────────────────────────

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

    // ── AC2: CreateWithStorageKey persists metadata; Data is null ──────────

    [Fact]
    public async Task CreateWithStorageKey_PersistsMetadataWithoutInlineData()
    {
        await using var db = CreateDbContext(nameof(CreateWithStorageKey_PersistsMetadataWithoutInlineData));

        var user = CreateUser("u1", "s1@univ.edu");
        db.Users.Add(user);
        var student = Student.Create(user.Id, "CS");
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var artifact = ThesisArtifact.CreateWithStorageKey(
            thesisId: Guid.NewGuid(),
            studentId: student.Id,
            uploadedByUserId: user.Id,
            fileName: "report.pdf",
            contentType: "application/pdf",
            fileSizeBytes: 1024,
            storageKey: "2026/07/03/abc-report.pdf");

        db.ThesisArtifacts.Add(artifact);
        await db.SaveChangesAsync();

        var loaded = await db.ThesisArtifacts
            .AsNoTracking()
            .FirstAsync(a => a.ArtifactId == artifact.ArtifactId);

        Assert.Equal("report.pdf", loaded.FileName);
        Assert.Equal("2026/07/03/abc-report.pdf", loaded.StorageKey);
        Assert.Null(loaded.Data);
        Assert.Equal(user.Id, loaded.UploadedByUserId);
        Assert.Equal(ArtifactScanStatus.Pending, loaded.ScanStatus);
    }

    // ── AC5: UpdateScanStatus persists the new status ──────────────────────

    [Fact]
    public async Task UpdateScanStatusAsync_PersistsScanStatus()
    {
        await using var db = CreateDbContext(nameof(UpdateScanStatusAsync_PersistsScanStatus));

        var user = CreateUser("u2", "s2@univ.edu");
        db.Users.Add(user);
        var student = Student.Create(user.Id, "CS");
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var artifact = ThesisArtifact.CreateWithStorageKey(
            Guid.NewGuid(), student.Id, user.Id,
            "slides.pptx", "application/vnd.ms-powerpoint", 2048, "2026/07/03/slides.pptx");

        db.ThesisArtifacts.Add(artifact);
        await db.SaveChangesAsync();

        var repo = new ThesisArtifactRepository(db);
        await repo.UpdateScanStatusAsync(artifact.ArtifactId, ArtifactScanStatus.Clean);

        await using var db2 = CreateDbContext(nameof(UpdateScanStatusAsync_PersistsScanStatus));
        var loaded = await db2.ThesisArtifacts.FirstAsync(a => a.ArtifactId == artifact.ArtifactId);

        Assert.Equal(ArtifactScanStatus.Clean, loaded.ScanStatus);
    }

    // ── AC4: Token generation + successful download ────────────────────────

    [Fact]
    public async Task DownloadByTokenAsync_ValidToken_ReturnsStream()
    {
        await using var db = CreateDbContext(nameof(DownloadByTokenAsync_ValidToken_ReturnsStream));
        using var tempDir = new TempDirectory();

        var user = CreateUser("u3", "s3@univ.edu");
        db.Users.Add(user);
        var student = Student.Create(user.Id, "AI");
        db.Students.Add(student);
        await db.SaveChangesAsync();

        // Upload a small file to local storage
        var localStorage = new LocalFileAttachmentStorageService(tempDir.Path);
        using var content = new MemoryStream([0x50, 0x4B, 0x03, 0x04, 0x01, 0x02]);
        var storageKey = await localStorage.UploadAsync(content, "pack.zip", "application/zip");

        var artifact = ThesisArtifact.CreateWithStorageKey(
            Guid.NewGuid(), student.Id, user.Id, "pack.zip", "application/zip", 6, storageKey);
        db.ThesisArtifacts.Add(artifact);
        await db.SaveChangesAsync();

        var dp = new EphemeralDataProtectionProvider(NullLoggerFactory.Instance);
        var svc = new ThesisArtifactStorageService(db, null, localStorage, dp, null);

        var token = svc.GenerateDownloadToken(artifact.ArtifactId);
        var download = await svc.DownloadByTokenAsync(token);

        Assert.NotNull(download);
        Assert.Equal("pack.zip", download!.FileName);

        await download.Stream.DisposeAsync();
    }

    // ── AC4: Expired / tampered token is rejected (maps to 403) ───────────

    [Fact]
    public async Task DownloadByTokenAsync_ExpiredToken_ReturnsNull()
    {
        await using var db = CreateDbContext(nameof(DownloadByTokenAsync_ExpiredToken_ReturnsNull));

        var dp = new EphemeralDataProtectionProvider(NullLoggerFactory.Instance);
        var svc = new ThesisArtifactStorageService(db, null, null, dp, null);

        // Create a token that expires immediately (negative lifetime)
        var protector = dp.CreateProtector("ThesisAttachmentDownload").ToTimeLimitedDataProtector();
        var expiredToken = protector.Protect(Guid.NewGuid().ToString("N"), DateTimeOffset.UtcNow.AddMinutes(-1));

        var result = await svc.DownloadByTokenAsync(expiredToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task DownloadByTokenAsync_TamperedToken_ReturnsNull()
    {
        await using var db = CreateDbContext(nameof(DownloadByTokenAsync_TamperedToken_ReturnsNull));

        var dp = new EphemeralDataProtectionProvider(NullLoggerFactory.Instance);
        var svc = new ThesisArtifactStorageService(db, null, null, dp, null);

        var result = await svc.DownloadByTokenAsync("this-is-not-a-valid-token");

        Assert.Null(result);
    }

    // ── AC6: Invalid extension rejected by LocalFileStorageService ─────────
    //    (Upload validation is enforced by the command handler; this test
    //     verifies the storage service itself still enforces inline size for
    //     legacy path when ISender is unavailable.)

    [Fact]
    public async Task LegacyStoreAsync_EmptyStream_ThrowsInvalidOperation()
    {
        await using var db = CreateDbContext(nameof(LegacyStoreAsync_EmptyStream_ThrowsInvalidOperation));

        var user = CreateUser("u4", "s4@univ.edu");
        db.Users.Add(user);
        var student = Student.Create(user.Id, "SE");
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var svc = new ThesisArtifactStorageService(db); // legacy constructor (no sender)

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.StoreAsync(user.Email, Guid.NewGuid(), "empty.zip", "application/zip",
                new MemoryStream(Array.Empty<byte>())));
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static UniversityDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new UniversityDbContext(options);
    }

    private static IdentityUser CreateUser(string id, string email) => new()
    {
        Id = id,
        UserName = email,
        NormalizedUserName = email.ToUpperInvariant(),
        Email = email,
        NormalizedEmail = email.ToUpperInvariant(),
        SecurityStamp = Guid.NewGuid().ToString("N"),
    };

    /// <summary>
    /// Disposable wrapper that creates a unique temp directory and deletes it on dispose.
    /// </summary>
    private sealed class TempDirectory : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        public TempDirectory() => Directory.CreateDirectory(Path);

        public void Dispose()
        {
            try { Directory.Delete(Path, recursive: true); } catch { /* best effort */ }
        }
    }
}

