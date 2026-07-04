namespace University.Application.Interfaces;

using University.Domain.Entities;
using University.Domain.Enums;

/// <summary>
/// Repository interface for <see cref="ThesisArtifact"/> persistence.
/// Implemented in the Infrastructure layer; consumed by Application command handlers.
/// </summary>
public interface IThesisArtifactRepository
{
    /// <summary>
    /// Returns the domain Student.Id for a given ASP.NET Core Identity user-id,
    /// or null if no student profile exists for that user.
    /// </summary>
    Task<int?> FindStudentIdByIdentityUserIdAsync(string identityUserId, CancellationToken ct = default);

    /// <summary>
    /// Persists a new <see cref="ThesisArtifact"/> and returns its public <see cref="Guid"/> identifier.
    /// </summary>
    Task<Guid> SaveAsync(ThesisArtifact artifact, CancellationToken ct = default);

    /// <summary>
    /// Returns the artifact with the given public identifier, or null if not found.
    /// </summary>
    Task<ThesisArtifact?> FindByArtifactIdAsync(Guid artifactId, CancellationToken ct = default);

    /// <summary>
    /// Updates the <see cref="ThesisArtifact.ScanStatus"/> for the specified artifact.
    /// No-op if the artifact does not exist.
    /// </summary>
    Task UpdateScanStatusAsync(Guid artifactId, ArtifactScanStatus status, CancellationToken ct = default);
}
