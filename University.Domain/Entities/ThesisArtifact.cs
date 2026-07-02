namespace University.Domain.Entities;

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
    /// Gets the binary payload.
    /// </summary>
    public byte[] Data { get; internal set; } = Array.Empty<byte>();

    /// <summary>
    /// Gets the upload timestamp.
    /// </summary>
    public DateTime UploadedOn { get; internal set; }

    /// <summary>
    /// Gets the navigation property to the owning student.
    /// </summary>
    public virtual Student? Student { get; private set; }

    /// <summary>
    /// Creates a persisted thesis artifact.
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
            UploadedOn = DateTime.UtcNow,
        };
    }
}