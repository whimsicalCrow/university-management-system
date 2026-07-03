namespace University.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using University.Application.Interfaces;
using University.Domain.Entities;
using University.Domain.Enums;
using University.Infrastructure.Data;

/// <summary>
/// EF Core–backed implementation of <see cref="IThesisArtifactRepository"/>.
/// </summary>
public sealed class ThesisArtifactRepository : IThesisArtifactRepository
{
    private readonly UniversityDbContext _db;

    public ThesisArtifactRepository(UniversityDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<int?> FindStudentIdByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default)
    {
        return await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == identityUserId)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(ct);
    }

    /// <inheritdoc />
    public async Task<Guid> SaveAsync(ThesisArtifact artifact, CancellationToken ct = default)
    {
        _db.ThesisArtifacts.Add(artifact);
        await _db.SaveChangesAsync(ct);
        return artifact.ArtifactId;
    }

    /// <inheritdoc />
    public async Task<ThesisArtifact?> FindByArtifactIdAsync(Guid artifactId, CancellationToken ct = default)
    {
        return await _db.ThesisArtifacts
            .FirstOrDefaultAsync(a => a.ArtifactId == artifactId, ct);
    }

    /// <inheritdoc />
    public async Task UpdateScanStatusAsync(Guid artifactId, ArtifactScanStatus status, CancellationToken ct = default)
    {
        var artifact = await _db.ThesisArtifacts
            .FirstOrDefaultAsync(a => a.ArtifactId == artifactId, ct);

        if (artifact is null)
            return;

        artifact.UpdateScanStatus(status);
        await _db.SaveChangesAsync(ct);
    }
}
