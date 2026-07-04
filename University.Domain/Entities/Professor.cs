namespace University.Domain.Entities;

using University.Domain.Exceptions;

/// <summary>
/// Represents a professor entity in the thesis collaboration portal.
/// Encapsulates professor information and relationships with students and feedback.
/// </summary>
public class Professor : BaseEntity
{
    /// <summary>
    /// Gets the user ID (foreign key to AspNetUser).
    /// </summary>
    public string UserId { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the professor's display name (e.g. "Επίκ. Χριστοδούλου").
    /// </summary>
    public string Name { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the department where the professor works.
    /// </summary>
    public string Department { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the professor's area of expertise.
    /// </summary>
    public string Expertise { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the collection of assignments for this professor.
    /// </summary>
    public virtual ICollection<Assignment> AssignedStudents { get; private set; } = new List<Assignment>();

    /// <summary>
    /// Gets the collection of feedback provided by this professor.
    /// </summary>
    public virtual ICollection<Feedback> FeedbackProvided { get; private set; } = new List<Feedback>();

    /// <summary>
    /// Creates a new professor instance with the specified user ID, department, and expertise.
    /// </summary>
    /// <param name="userId">The user ID (foreign key to AspNetUser).</param>
    /// <param name="department">The department where the professor works.</param>
    /// <param name="expertise">The professor's area of expertise.</param>
    /// <returns>A new Professor instance.</returns>
    /// <exception cref="DomainException">Thrown when any parameter is null or empty.</exception>
    public static Professor Create(string userId, string department, string expertise)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(department))
            throw new DomainException("Department cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(expertise))
            throw new DomainException("Expertise cannot be null or empty.");

        return new Professor
        {
            UserId = userId,
            Department = department,
            Expertise = expertise
        };
    }

    /// <summary>
    /// Provides feedback on a thesis update.
    /// </summary>
    /// <param name="updateId">The ID of the thesis update being reviewed.</param>
    /// <param name="comment">The feedback comment.</param>
    /// <returns>The created Feedback entity.</returns>
    /// <exception cref="DomainException">Thrown when comment is null, empty, or exceeds 5000 characters.</exception>
    public Feedback ProvideFeedback(int updateId, string comment)
    {
        var feedback = Feedback.Create(updateId, Id, comment);
        FeedbackProvided.Add(feedback);
        return feedback;
    }
}
