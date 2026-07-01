namespace University.Domain.Entities;

using University.Domain.Exceptions;

/// <summary>
/// Represents feedback provided by a professor on a student's thesis update.
/// Captures the professor's comments and timestamp for academic review.
/// </summary>
public class Feedback : BaseEntity
{
    /// <summary>
    /// Gets the ID of the thesis update being reviewed (foreign key to ThesisUpdate).
    /// </summary>
    public int UpdateId { get; private set; }

    /// <summary>
    /// Gets the ID of the professor providing feedback (foreign key to Professor).
    /// </summary>
    public int ProfessorId { get; private set; }

    /// <summary>
    /// Gets the feedback comment.
    /// </summary>
    public string Comment { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the feedback was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; private set; }

    /// <summary>
    /// Gets the navigation property to the thesis update.
    /// </summary>
    public virtual ThesisUpdate? Update { get; private set; }

    /// <summary>
    /// Gets the navigation property to the professor who provided feedback.
    /// </summary>
    public virtual Professor? Professor { get; private set; }

    /// <summary>
    /// Creates new feedback on a thesis update.
    /// </summary>
    /// <param name="updateId">The ID of the thesis update being reviewed.</param>
    /// <param name="professorId">The ID of the professor providing feedback.</param>
    /// <param name="comment">The feedback comment.</param>
    /// <returns>A new Feedback instance.</returns>
    /// <exception cref="DomainException">Thrown when parameters are invalid or comment exceeds character limit.</exception>
    public static Feedback Create(int updateId, int professorId, string comment)
    {
        if (updateId <= 0)
            throw new DomainException("UpdateId must be greater than zero.");

        if (professorId <= 0)
            throw new DomainException("ProfessorId must be greater than zero.");

        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Feedback comment cannot be null or empty.");

        if (comment.Length > 5000)
            throw new DomainException("Feedback comment cannot exceed 5000 characters.");

        return new Feedback
        {
            UpdateId = updateId,
            ProfessorId = professorId,
            Comment = comment,
            SubmittedAt = DateTime.UtcNow
        };
    }
}
