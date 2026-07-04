namespace University.Domain.Entities;

using University.Domain.Enums;
using University.Domain.Exceptions;

/// <summary>
/// Represents a thesis update submission by a student.
/// Tracks the progress of student thesis work with status tracking and feedback collection.
/// </summary>
public class ThesisUpdate : BaseEntity
{
    /// <summary>
    /// Gets the ID of the student who submitted the update (foreign key to Student).
    /// </summary>
    public int StudentId { get; internal set; }

    /// <summary>
    /// Gets the content of the thesis update.
    /// </summary>
    public string Content { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the update was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; internal set; }

    /// <summary>
    /// Gets the short title of this update (displayed as the timeline card heading).
    /// Null for legacy records created before this field was introduced.
    /// </summary>
    public string? Title { get; internal set; }

    /// <summary>
    /// Gets the optional attachment file name for this update.
    /// </summary>
    public string? AttachmentFileName { get; internal set; }

    /// <summary>
    /// Gets the optional attachment file size in bytes.
    /// </summary>
    public long? AttachmentSizeBytes { get; internal set; }

    /// <summary>
    /// Gets the public identifier of the <see cref="ThesisArtifact"/> linked to this update, if any.
    /// Stored as a foreign key reference (Guid) to the ThesisArtifacts.ArtifactId column.
    /// </summary>
    public Guid? ThesisArtifactId { get; internal set; }

    /// <summary>
    /// Gets the current status of the update (Draft, Submitted, or Reviewed).
    /// </summary>
    public UpdateStatus Status { get; internal set; }

    /// <summary>
    /// Gets the navigation property to the student who submitted this update.
    /// </summary>
    public virtual Student? Student { get; private set; }

    /// <summary>
    /// Gets the collection of feedback provided on this update.
    /// </summary>
    public virtual ICollection<Feedback> Feedback { get; private set; } = new List<Feedback>();

    /// <summary>
    /// Creates a new thesis update with the specified content.
    /// </summary>
    /// <param name="studentId">The ID of the student submitting the update.</param>
    /// <param name="content">The content of the update.</param>
    /// <returns>A new ThesisUpdate instance.</returns>
    /// <exception cref="DomainException">Thrown when studentId is invalid or content is invalid.</exception>
    /// <param name="title">Optional short title for the timeline card heading (max 300 chars).</param>
    public static ThesisUpdate Create(int studentId, string content, string? title = null, UpdateStatus status = UpdateStatus.Draft)
    {
        if (studentId <= 0)
            throw new DomainException("StudentId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Update content cannot be null or empty.");

        if (content.Length > 10000)
            throw new DomainException("Update content cannot exceed 10000 characters.");

        if (title is not null && title.Length > 300)
            throw new DomainException("Update title cannot exceed 300 characters.");

        return new ThesisUpdate
        {
            StudentId = studentId,
            Content = content,
            Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
            SubmittedAt = DateTime.UtcNow,
            Status = status
        };
    }

    /// <summary>
    /// Submits the update for review, changing its status from Draft to Submitted.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the update is not in Draft status.</exception>
    public void Submit()
    {
        if (Status != UpdateStatus.Draft)
            throw new DomainException($"Update must be in Draft status to submit. Current status: {Status}");

        Status = UpdateStatus.Submitted;
    }

    /// <summary>
    /// Marks the update as reviewed.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the update is not in Submitted status.</exception>
    public void MarkAsReviewed()
    {
        if (Status != UpdateStatus.Submitted)
            throw new DomainException($"Update must be in Submitted status to mark as reviewed. Current status: {Status}");

        Status = UpdateStatus.Reviewed;
    }

    /// <summary>
    /// Attaches metadata for a submitted artifact.
    /// </summary>
    /// <param name="fileName">Attachment filename.</param>
    /// <param name="sizeBytes">Attachment size in bytes.</param>
    public void AttachFileMetadata(string fileName, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("Attachment filename cannot be null or empty.");

        if (sizeBytes <= 0)
            throw new DomainException("Attachment size must be greater than zero.");

        AttachmentFileName = fileName.Trim();
        AttachmentSizeBytes = sizeBytes;
    }

    /// <summary>
    /// Links a persisted <see cref="ThesisArtifact"/> to this update so the timeline can
    /// generate a download token for the attached file.
    /// </summary>
    public void LinkArtifact(Guid artifactId)
    {
        if (artifactId == Guid.Empty)
            throw new DomainException("ArtifactId must not be empty.");

        ThesisArtifactId = artifactId;
    }

    /// <summary>
    /// Transitions the update back to Submitted status, indicating the student must revise
    /// before the next review cycle.
    /// NOTE: We reuse Submitted (not a new enum value) to avoid a DB migration for the
    /// presentation demo — this matches the story's status-mapping decision.
    /// </summary>
    public void RequestRevision()
    {
        // Allow from either Reviewed or Submitted so the professor can request further
        // revision even after multiple review cycles.
        if (Status == UpdateStatus.Draft)
            throw new DomainException("Cannot request revision on a Draft update.");

        Status = UpdateStatus.Submitted;
    }

    /// <summary>
    /// Updates the editable content fields of a Draft or Submitted update.
    /// Reviewed updates cannot be edited.
    /// </summary>
    public void UpdateContent(string content, string? title)
    {
        if (Status == UpdateStatus.Reviewed)
            throw new DomainException("Cannot edit a Reviewed update.");

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Update content cannot be null or empty.");

        if (content.Length > 10000)
            throw new DomainException("Update content cannot exceed 10000 characters.");

        if (title is not null && title.Length > 300)
            throw new DomainException("Update title cannot exceed 300 characters.");

        Content = content;
        Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
    }

    /// <summary>
    /// Transitions status to the given value, provided the update is not already Reviewed.
    /// Used by the service layer when persisting draft / submit transitions.
    /// </summary>
    public void SetStatus(UpdateStatus newStatus)
    {
        if (Status != UpdateStatus.Reviewed)
            Status = newStatus;
    }
}
