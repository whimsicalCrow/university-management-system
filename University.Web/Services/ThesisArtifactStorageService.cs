using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using University.Application.Commands;
using University.Application.Interfaces;
using University.Domain.Entities;
using University.Infrastructure.Data;

namespace University.Web.Services;

public interface IThesisArtifactStorageService
{
    Task<Guid> StoreAsync(
        string? userName,
        Guid thesisId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken = default);

    Task<StoredThesisArtifact?> GetByArtifactIdAsync(Guid artifactId, CancellationToken cancellationToken = default);

    Task<StoredThesisArtifact?> GetOwnedByArtifactIdAsync(string identityUserId, Guid artifactId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a time-limited signed download token for the given artifact.
    /// Falls back to the raw artifact-id string when data protection is unavailable
    /// (e.g., in legacy test setups).
    /// </summary>
    string GenerateDownloadToken(Guid artifactId);

    /// <summary>
    /// Validates <paramref name="token"/>, looks up the artifact, and returns a
    /// <see cref="DownloadedArtifact"/> ready for streaming. Returns <c>null</c> when
    /// the token is expired, tampered, or the artifact cannot be found.
    /// </summary>
    Task<DownloadedArtifact?> DownloadByTokenAsync(string token, CancellationToken cancellationToken = default);
}

public sealed record StoredThesisArtifact(
    Guid ArtifactId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    byte[]? Data);

public sealed record DownloadedArtifact(Stream Stream, string ContentType, string FileName);

public sealed class ThesisArtifactStorageService : IThesisArtifactStorageService
{
    private const long LegacyMaxArtifactSizeBytes = 25 * 1024 * 1024;
    private const string DataProtectionPurpose = "ThesisAttachmentDownload";

    private readonly UniversityDbContext _db;
    private readonly ISender? _sender;
    private readonly IAttachmentStorageService? _attachmentStorage;
    private readonly IDataProtectionProvider? _dataProtectionProvider;
    private readonly IConfiguration? _configuration;

    // ── Constructors ──────────────────────────────────────────────────────

    /// <summary>
    /// Legacy constructor for backward compatibility (used by existing integration tests).
    /// Upload falls back to inline database storage; token generation returns the raw artifact-id.
    /// </summary>
    public ThesisArtifactStorageService(UniversityDbContext db)
        : this(db, null, null, null, null)
    {
    }

    /// <summary>
    /// Full-pipeline constructor used by the DI container in production.
    /// </summary>
    public ThesisArtifactStorageService(
        UniversityDbContext db,
        ISender? sender,
        IAttachmentStorageService? attachmentStorage,
        IDataProtectionProvider? dataProtectionProvider,
        IConfiguration? configuration)
    {
        _db = db;
        _sender = sender;
        _attachmentStorage = attachmentStorage;
        _dataProtectionProvider = dataProtectionProvider;
        _configuration = configuration;
    }

    // ── Upload ────────────────────────────────────────────────────────────

    public async Task<Guid> StoreAsync(
        string? userName,
        Guid thesisId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        // New pipeline: dispatch command to Application layer handler
        if (_sender is not null)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new InvalidOperationException("Authenticated student session is required for artifact upload.");

            var command = new UploadAttachmentCommand
            {
                IdentityUserName = userName.Trim(),
                ThesisId = thesisId,
                FileName = fileName,
                ContentType = contentType,
                FileStream = content,
            };

            var result = await _sender.Send(command, cancellationToken);

            if (!result.Success)
                throw new InvalidOperationException(result.Error ?? "Attachment upload failed.");

            return result.ArtifactId;
        }

        // Legacy fallback: store binary inline in the database
        return await LegacyStoreAsync(userName, thesisId, fileName, contentType, content, cancellationToken);
    }

    // ── Read ──────────────────────────────────────────────────────────────

    public async Task<StoredThesisArtifact?> GetByArtifactIdAsync(Guid artifactId, CancellationToken cancellationToken = default)
    {
        return await _db.ThesisArtifacts
            .AsNoTracking()
            .Where(a => a.ArtifactId == artifactId)
            .Select(a => new StoredThesisArtifact(
                a.ArtifactId,
                a.FileName,
                a.ContentType,
                a.FileSizeBytes,
                a.Data))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<StoredThesisArtifact?> GetOwnedByArtifactIdAsync(string identityUserId, Guid artifactId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
            return null;

        var studentId = await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == identityUserId)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!studentId.HasValue)
            return null;

        return await _db.ThesisArtifacts
            .AsNoTracking()
            .Where(a => a.ArtifactId == artifactId && a.StudentId == studentId.Value)
            .Select(a => new StoredThesisArtifact(
                a.ArtifactId,
                a.FileName,
                a.ContentType,
                a.FileSizeBytes,
                a.Data))
            .FirstOrDefaultAsync(cancellationToken);
    }

    // ── Signed download tokens ────────────────────────────────────────────

    public string GenerateDownloadToken(Guid artifactId)
    {
        if (_dataProtectionProvider is null)
            return artifactId.ToString("N"); // legacy fallback for tests / unconfigured environments

        var expiryMinutes = _configuration?.GetValue<int>("Attachments:DownloadTokenExpiryMinutes", 15) ?? 15;

        return _dataProtectionProvider
            .CreateProtector(DataProtectionPurpose)
            .ToTimeLimitedDataProtector()
            .Protect(artifactId.ToString("N"), TimeSpan.FromMinutes(expiryMinutes));
    }

    public async Task<DownloadedArtifact?> DownloadByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        Guid artifactId;

        if (_dataProtectionProvider is null)
        {
            // Legacy / test path: GenerateDownloadToken returns raw Guid("N") when DP unavailable
            if (!Guid.TryParseExact(token, "N", out artifactId))
                return null;
        }
        else
        {
            try
            {
                var payload = _dataProtectionProvider
                    .CreateProtector(DataProtectionPurpose)
                    .ToTimeLimitedDataProtector()
                    .Unprotect(token, out _); // throws CryptographicException if expired or tampered

                artifactId = Guid.ParseExact(payload, "N");
            }
            catch
            {
                return null; // expired or tampered token
            }
        }

        var artifact = await _db.ThesisArtifacts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ArtifactId == artifactId, cancellationToken);

        if (artifact is null)
            return null;

        // New pipeline: serve from external storage provider
        if (!string.IsNullOrEmpty(artifact.StorageKey) && _attachmentStorage is not null)
        {
            var (stream, _) = await _attachmentStorage.DownloadAsync(artifact.StorageKey, cancellationToken);
            // Use the ContentType stored in the DB; the local storage provider has no type information
            return new DownloadedArtifact(stream, artifact.ContentType, artifact.FileName);
        }

        // Legacy: serve inline bytes from the database
        if (artifact.Data is { Length: > 0 })
            return new DownloadedArtifact(new MemoryStream(artifact.Data), artifact.ContentType, artifact.FileName);

        return null;
    }

    // ── Legacy helpers ────────────────────────────────────────────────────

    private async Task<Guid> LegacyStoreAsync(
        string? userName,
        Guid thesisId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new InvalidOperationException("Authenticated student session is required for artifact upload.");

        var normalizedUser = userName.Trim().ToUpperInvariant();
        var identityUser = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.NormalizedEmail == normalizedUser || u.NormalizedUserName == normalizedUser,
                cancellationToken);

        if (identityUser is null)
            throw new InvalidOperationException("Unable to resolve current user for artifact storage.");

        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == identityUser.Id, cancellationToken);

        if (student is null)
            throw new InvalidOperationException("Only student users can upload thesis artifacts.");

        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);

        if (buffer.Length == 0)
            throw new InvalidOperationException("Uploaded file content is empty.");

        if (buffer.Length > LegacyMaxArtifactSizeBytes)
            throw new InvalidOperationException($"Uploaded file exceeds the limit of {LegacyMaxArtifactSizeBytes} bytes.");

        var artifact = ThesisArtifact.Create(
            thesisId,
            student.Id,
            fileName,
            string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            buffer.ToArray());

        _db.ThesisArtifacts.Add(artifact);
        await _db.SaveChangesAsync(cancellationToken);

        return artifact.ArtifactId;
    }
}
