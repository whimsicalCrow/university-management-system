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
    public static ThesisUpdate Create(int studentId, string content)
    {
        if (studentId <= 0)
            throw new DomainException("StudentId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Update content cannot be null or empty.");

        if (content.Length > 10000)
            throw new DomainException("Update content cannot exceed 10000 characters.");

        return new ThesisUpdate
        {
            StudentId = studentId,
            Content = content,
            SubmittedAt = DateTime.UtcNow,
            Status = UpdateStatus.Draft
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
}
