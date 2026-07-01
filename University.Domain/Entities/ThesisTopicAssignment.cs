namespace University.Domain.Entities;

using University.Domain.Exceptions;

/// <summary>
/// Represents a persisted assignment of a thesis topic to a student and professor.
/// </summary>
public class ThesisTopicAssignment : BaseEntity
{
    /// <summary>
    /// Gets the deterministic topic identifier from the UI catalog.
    /// </summary>
    public Guid TopicId { get; internal set; }

    /// <summary>
    /// Gets the thesis topic title captured at assignment time.
    /// </summary>
    public string TopicTitle { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the assignment status.
    /// </summary>
    public string Status { get; internal set; } = "Ongoing";

    /// <summary>
    /// Gets the assigned student id.
    /// </summary>
    public int StudentId { get; internal set; }

    /// <summary>
    /// Gets the supervising professor id.
    /// </summary>
    public int ProfessorId { get; internal set; }

    /// <summary>
    /// Gets when this assignment was created.
    /// </summary>
    public DateTime AssignedOn { get; internal set; }

    /// <summary>
    /// Gets navigation property to student.
    /// </summary>
    public virtual Student? Student { get; private set; }

    /// <summary>
    /// Gets navigation property to professor.
    /// </summary>
    public virtual Professor? Professor { get; private set; }

    /// <summary>
    /// Creates a new ongoing topic assignment.
    /// </summary>
    public static ThesisTopicAssignment Create(Guid topicId, string topicTitle, int studentId, int professorId)
    {
        if (topicId == Guid.Empty)
            throw new DomainException("TopicId is required.");

        if (string.IsNullOrWhiteSpace(topicTitle))
            throw new DomainException("TopicTitle is required.");

        if (studentId <= 0)
            throw new DomainException("StudentId must be greater than zero.");

        if (professorId <= 0)
            throw new DomainException("ProfessorId must be greater than zero.");

        return new ThesisTopicAssignment
        {
            TopicId = topicId,
            TopicTitle = topicTitle.Trim(),
            Status = "Ongoing",
            StudentId = studentId,
            ProfessorId = professorId,
            AssignedOn = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Updates assignment metadata while keeping it ongoing.
    /// </summary>
    public void Refresh(string topicTitle, int studentId, int professorId)
    {
        if (string.IsNullOrWhiteSpace(topicTitle))
            throw new DomainException("TopicTitle is required.");

        if (studentId <= 0)
            throw new DomainException("StudentId must be greater than zero.");

        if (professorId <= 0)
            throw new DomainException("ProfessorId must be greater than zero.");

        TopicTitle = topicTitle.Trim();
        StudentId = studentId;
        ProfessorId = professorId;
        Status = "Ongoing";
    }
}