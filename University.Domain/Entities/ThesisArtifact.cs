namespace University.Domain.Entities;

using University.Domain.Enums;
using University.Domain.Exceptions;

/// <summary>
/// Represents a persisted thesis artifact uploaded by a student.
/// </summary>
public class ThesisArtifact : BaseEntity
{
    /// <summary>
    /// Gets the public artifact identifier.
    /// </summary>
    public Guid ArtifactId { get; internal set; }

    /// <summary>
    /// Gets the thesis identifier associated with the artifact.
    /// </summary>
    public Guid ThesisId { get; internal set; }

    /// <summary>
    /// Gets the owning student identifier.
    /// </summary>
    public int StudentId { get; internal set; }

    /// <summary>
    /// Gets the original file name.
    /// </summary>
    public string FileName { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the uploaded MIME content type.
    /// </summary>
    public string ContentType { get; internal set; } = "application/octet-stream";

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public long FileSizeBytes { get; internal set; }

    /// <summary>
    /// Gets the binary payload (legacy in-database storage; null for storage-pipeline artifacts).
    /// </summary>
    public byte[]? Data { get; internal set; }

    /// <summary>
    /// Gets the opaque storage key used to retrieve the binary from the configured storage provider.
    /// Null for legacy artifacts that were stored inline in the database.
    /// </summary>
    public string? StorageKey { get; internal set; }

    /// <summary>
    /// Gets the Identity user-id of the student who uploaded the artifact.
    /// </summary>
    public string? UploadedByUserId { get; internal set; }

    /// <summary>
    /// Gets the result of the virus scan pipeline for this artifact.
    /// </summary>
    public ArtifactScanStatus ScanStatus { get; internal set; } = ArtifactScanStatus.Skipped;

    /// <summary>
    /// Gets the upload timestamp.
    /// </summary>
    public DateTime UploadedOn { get; internal set; }

    /// <summary>
    /// Gets the navigation property to the owning student.
    /// </summary>
    public virtual Student? Student { get; private set; }

    // ── Factories ─────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a legacy thesis artifact that stores binary content directly in the database.
    /// </summary>
    public static ThesisArtifact Create(
        Guid thesisId,
        int studentId,
        string fileName,
        string contentType,
        byte[] data)
    {
        if (thesisId == Guid.Empty)
            throw new DomainException("ThesisId is required.");

        if (studentId <= 0)
            throw new DomainException("StudentId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("FileName is required.");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException("ContentType is required.");

        if (data is null || data.Length == 0)
            throw new DomainException("Artifact data cannot be empty.");

        return new ThesisArtifact
        {
            ArtifactId = Guid.NewGuid(),
            ThesisId = thesisId,
            StudentId = studentId,
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            FileSizeBytes = data.LongLength,
            Data = data,
            ScanStatus = ArtifactScanStatus.Skipped,
            UploadedOn = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Creates a thesis artifact using the storage-pipeline approach: binary is held in an
    /// external provider identified by <paramref name="storageKey"/>.
    /// </summary>
    public static ThesisArtifact CreateWithStorageKey(
        Guid thesisId,
        int studentId,
        string uploadedByUserId,
        string fileName,
        string contentType,
        long fileSizeBytes,
        string storageKey)
    {
        if (thesisId == Guid.Empty)
            throw new DomainException("ThesisId is required.");

        if (studentId <= 0)
            throw new DomainException("StudentId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(uploadedByUserId))
            throw new DomainException("UploadedByUserId is required.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("FileName is required.");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException("ContentType is required.");

        if (fileSizeBytes <= 0)
            throw new DomainException("FileSizeBytes must be greater than zero.");

        if (string.IsNullOrWhiteSpace(storageKey))
            throw new DomainException("StorageKey is required.");

        return new ThesisArtifact
        {
            ArtifactId = Guid.NewGuid(),
            ThesisId = thesisId,
            StudentId = studentId,
            UploadedByUserId = uploadedByUserId.Trim(),
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            FileSizeBytes = fileSizeBytes,
            StorageKey = storageKey.Trim(),
            ScanStatus = ArtifactScanStatus.Pending,
            UploadedOn = DateTime.UtcNow,
        };
    }

    // ── Mutations ──────────────────────────────────────────────────────────

    /// <summary>
    /// Records the outcome of the virus scan pipeline.
    /// </summary>
    public void UpdateScanStatus(ArtifactScanStatus status)
    {
        ScanStatus = status;
    }
}