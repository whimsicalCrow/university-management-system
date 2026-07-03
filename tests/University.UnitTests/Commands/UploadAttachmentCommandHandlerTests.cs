namespace University.UnitTests.Commands;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using University.Application.Commands;
using University.Application.Interfaces;
using University.Domain.Entities;
using University.Domain.Enums;

/// <summary>
/// Unit tests for <see cref="UploadAttachmentCommandHandler"/>.
/// All dependencies are replaced by lightweight in-memory stubs; no database required.
/// </summary>
public class UploadAttachmentCommandHandlerTests
{
    // ── Magic bytes ─────────────────────────────────────────────────────────
    private static readonly byte[] ValidZipBytes = [0x50, 0x4B, 0x03, 0x04, 0x00, 0x00]; // PK\x03\x04
    private static readonly byte[] ValidPdfBytes = [0x25, 0x50, 0x44, 0x46, 0x2D];       // %PDF-

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static IConfiguration BuildConfig(
        string[]? allowedExtensions = null,
        long? maxFileSizeBytes = null)
    {
        var dict = new Dictionary<string, string?>();

        if (allowedExtensions is not null)
        {
            for (var i = 0; i < allowedExtensions.Length; i++)
                dict[$"Attachments:AllowedExtensions:{i}"] = allowedExtensions[i];
        }

        if (maxFileSizeBytes.HasValue)
            dict["Attachments:MaxFileSizeBytes"] = maxFileSizeBytes.Value.ToString();

        return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
    }

    private static UploadAttachmentCommandHandler BuildHandler(
        IConfiguration? config = null,
        IThesisArtifactRepository? repo = null,
        IAttachmentStorageService? storage = null,
        IVirusScanService? virusScan = null,
        FakeUserManager? userManager = null)
    {
        return new UploadAttachmentCommandHandler(
            userManager ?? FakeUserManager.CreateDefault("student1@univ.edu", "user-id-1"),
            repo ?? new FakeThesisArtifactRepository(studentId: 10),
            storage ?? new FakeAttachmentStorageService(),
            virusScan ?? new FakeVirusScanService(VirusScanResult.Skipped),
            config ?? BuildConfig(),
            NullLogger<UploadAttachmentCommandHandler>.Instance);
    }

    // ── AC: invalid extension ──────────────────────────────────────────────

    [Theory]
    [InlineData(".exe")]
    [InlineData(".bat")]
    [InlineData(".js")]
    public async Task Handle_InvalidExtension_ReturnsError(string extension)
    {
        var handler = BuildHandler(config: BuildConfig(allowedExtensions: [".pdf", ".zip"]));
        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "student1@univ.edu",
            ThesisId = Guid.NewGuid(),
            FileName = $"file{extension}",
            ContentType = "application/octet-stream",
            FileStream = new MemoryStream(ValidZipBytes),
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Contains(extension, result.Error, StringComparison.OrdinalIgnoreCase);
    }

    // ── AC: file too large ─────────────────────────────────────────────────

    [Fact]
    public async Task Handle_FileTooLarge_ReturnsError()
    {
        var handler = BuildHandler(config: BuildConfig(maxFileSizeBytes: 4)); // only 4 bytes allowed
        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "student1@univ.edu",
            ThesisId = Guid.NewGuid(),
            FileName = "big.zip",
            ContentType = "application/zip",
            FileStream = new MemoryStream(ValidZipBytes), // 6 bytes > 4
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Contains("maximum", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    // ── AC: magic byte mismatch ────────────────────────────────────────────

    [Fact]
    public async Task Handle_MagicByteMismatch_ReturnsError()
    {
        var handler = BuildHandler();
        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "student1@univ.edu",
            ThesisId = Guid.NewGuid(),
            FileName = "report.pdf",               // .pdf extension
            ContentType = "application/pdf",
            FileStream = new MemoryStream(ValidZipBytes), // ZIP magic bytes, not PDF
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Contains(".pdf", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    // ── AC: valid upload persists artifact and returns ArtifactId ──────────

    [Fact]
    public async Task Handle_ValidZipUpload_PersistsArtifactAndReturnsId()
    {
        var repo = new FakeThesisArtifactRepository(studentId: 7);
        var storage = new FakeAttachmentStorageService();
        var handler = BuildHandler(repo: repo, storage: storage);

        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "student1@univ.edu",
            ThesisId = Guid.NewGuid(),
            FileName = "thesis-pack.zip",
            ContentType = "application/zip",
            FileStream = new MemoryStream(ValidZipBytes),
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.ArtifactId);
        Assert.Single(repo.Saved);
        Assert.False(string.IsNullOrEmpty(storage.LastStorageKey));
    }

    [Fact]
    public async Task Handle_ValidPdfUpload_PersistsArtifactAndReturnsId()
    {
        var repo = new FakeThesisArtifactRepository(studentId: 7);
        var handler = BuildHandler(repo: repo);

        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "student1@univ.edu",
            ThesisId = Guid.NewGuid(),
            FileName = "report.pdf",
            ContentType = "application/pdf",
            FileStream = new MemoryStream(ValidPdfBytes),
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.ArtifactId);
    }

    // ── AC: virus scan hook updates ScanStatus ─────────────────────────────

    [Fact]
    public async Task Handle_VirusScanReturnsClean_ScanStatusIsClean()
    {
        var repo = new FakeThesisArtifactRepository(studentId: 7);
        var virusScan = new FakeVirusScanService(VirusScanResult.Clean);
        var handler = BuildHandler(repo: repo, virusScan: virusScan);

        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "student1@univ.edu",
            ThesisId = Guid.NewGuid(),
            FileName = "docs.zip",
            ContentType = "application/zip",
            FileStream = new MemoryStream(ValidZipBytes),
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Single(repo.ScanStatusUpdates);
        Assert.Equal(ArtifactScanStatus.Clean, repo.ScanStatusUpdates.Single().Value);
    }

    [Fact]
    public async Task Handle_VirusScanReturnsInfected_ReturnsError()
    {
        var repo = new FakeThesisArtifactRepository(studentId: 7);
        var virusScan = new FakeVirusScanService(VirusScanResult.Infected);
        var handler = BuildHandler(repo: repo, virusScan: virusScan);

        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "student1@univ.edu",
            ThesisId = Guid.NewGuid(),
            FileName = "malware.zip",
            ContentType = "application/zip",
            FileStream = new MemoryStream(ValidZipBytes),
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("scanner", result.Error, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(ArtifactScanStatus.Infected, repo.ScanStatusUpdates.Single().Value);
    }

    // ── AC: unknown user ───────────────────────────────────────────────────

    [Fact]
    public async Task Handle_UnknownUser_ReturnsError()
    {
        var um = FakeUserManager.CreateEmpty(); // returns null for any lookup
        var handler = BuildHandler(userManager: um);

        var command = new UploadAttachmentCommand
        {
            IdentityUserName = "nobody@unknown.com",
            ThesisId = Guid.NewGuid(),
            FileName = "doc.zip",
            ContentType = "application/zip",
            FileStream = new MemoryStream(ValidZipBytes),
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Contains("user", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    // ══════════════════════════════════════════════════════════════════════
    // Test doubles
    // ══════════════════════════════════════════════════════════════════════

    private sealed class FakeAttachmentStorageService : IAttachmentStorageService
    {
        public string? LastStorageKey { get; private set; }
        private readonly Dictionary<string, byte[]> _store = new();

        public Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            var key = $"fake/{Guid.NewGuid():N}-{fileName}";
            _store[key] = ms.ToArray();
            LastStorageKey = key;
            return Task.FromResult(key);
        }

        public Task<(Stream Stream, string ContentType)> DownloadAsync(string storageKey, CancellationToken ct = default)
        {
            if (!_store.TryGetValue(storageKey, out var data))
                throw new FileNotFoundException(storageKey);
            return Task.FromResult(((Stream)new MemoryStream(data), "application/octet-stream"));
        }

        public Task DeleteAsync(string storageKey, CancellationToken ct = default)
        {
            _store.Remove(storageKey);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeVirusScanService(VirusScanResult result) : IVirusScanService
    {
        public Task<VirusScanResult> ScanAsync(string storageKey, CancellationToken ct = default) =>
            Task.FromResult(result);
    }

    private sealed class FakeThesisArtifactRepository(int studentId) : IThesisArtifactRepository
    {
        public List<ThesisArtifact> Saved { get; } = new();
        public Dictionary<Guid, ArtifactScanStatus> ScanStatusUpdates { get; } = new();

        public Task<int?> FindStudentIdByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default) =>
            Task.FromResult<int?>(studentId);

        public Task<Guid> SaveAsync(ThesisArtifact artifact, CancellationToken ct = default)
        {
            Saved.Add(artifact);
            return Task.FromResult(artifact.ArtifactId);
        }

        public Task<ThesisArtifact?> FindByArtifactIdAsync(Guid artifactId, CancellationToken ct = default) =>
            Task.FromResult(Saved.FirstOrDefault(a => a.ArtifactId == artifactId));

        public Task UpdateScanStatusAsync(Guid artifactId, ArtifactScanStatus status, CancellationToken ct = default)
        {
            ScanStatusUpdates[artifactId] = status;
            // Also update the in-memory artifact for verification
            Saved.FirstOrDefault(a => a.ArtifactId == artifactId)?.UpdateScanStatus(status);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUserManager : UserManager<IdentityUser>
    {
        private readonly IdentityUser? _user;

        private FakeUserManager(IdentityUser? user)
            : base(
                new FakeUserStore(),
                null!, null!, null!, null!, null!, null!, null!, null!)
        {
            _user = user;
        }

        public static FakeUserManager CreateDefault(string email, string id) =>
            new(new IdentityUser { Id = id, Email = email, UserName = email, NormalizedEmail = email.ToUpperInvariant() });

        public static FakeUserManager CreateEmpty() => new(null);

        public override Task<IdentityUser?> FindByEmailAsync(string email) =>
            Task.FromResult(_user?.Email?.Equals(email, StringComparison.OrdinalIgnoreCase) == true ? _user : null);

        public override Task<IdentityUser?> FindByNameAsync(string userName) =>
            Task.FromResult(_user?.UserName?.Equals(userName, StringComparison.OrdinalIgnoreCase) == true ? _user : null);

        private sealed class FakeUserStore : IUserStore<IdentityUser>
        {
            public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken ct) => Task.FromResult(IdentityResult.Success);
            public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken ct) => Task.FromResult(IdentityResult.Success);
            public void Dispose() { }
            public Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken ct) => Task.FromResult<IdentityUser?>(null);
            public Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken ct) => Task.FromResult<IdentityUser?>(null);
            public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken ct) => Task.FromResult(user.NormalizedUserName);
            public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken ct) => Task.FromResult(user.Id);
            public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken ct) => Task.FromResult(user.UserName);
            public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken ct) { user.NormalizedUserName = normalizedName; return Task.CompletedTask; }
            public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken ct) { user.UserName = userName; return Task.CompletedTask; }
            public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken ct) => Task.FromResult(IdentityResult.Success);
        }
    }
}
