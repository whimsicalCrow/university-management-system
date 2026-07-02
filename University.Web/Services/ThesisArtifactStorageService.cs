using Microsoft.EntityFrameworkCore;
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
}

public sealed record StoredThesisArtifact(
    Guid ArtifactId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    byte[] Data);

public sealed class ThesisArtifactStorageService : IThesisArtifactStorageService
{
    private const long MaxArtifactSizeBytes = 25 * 1024 * 1024;
    private readonly UniversityDbContext _db;

    public ThesisArtifactStorageService(UniversityDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> StoreAsync(
        string? userName,
        Guid thesisId,
        string fileName,
        string contentType,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new InvalidOperationException("Authenticated student session is required for artifact upload.");
        }

        var normalizedUser = userName.Trim().ToUpperInvariant();
        var identityUser = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.NormalizedEmail == normalizedUser || u.NormalizedUserName == normalizedUser,
                cancellationToken);

        if (identityUser is null)
        {
            throw new InvalidOperationException("Unable to resolve current user for artifact storage.");
        }

        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == identityUser.Id, cancellationToken);

        if (student is null)
        {
            throw new InvalidOperationException("Only student users can upload thesis artifacts.");
        }

        await using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);

        if (buffer.Length == 0)
        {
            throw new InvalidOperationException("Uploaded file content is empty.");
        }

        if (buffer.Length > MaxArtifactSizeBytes)
        {
            throw new InvalidOperationException($"Uploaded file exceeds the limit of {MaxArtifactSizeBytes} bytes.");
        }

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
        {
            return null;
        }

        var studentId = await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == identityUserId)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!studentId.HasValue)
        {
            return null;
        }

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
}