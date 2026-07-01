namespace University.Domain.Entities;

using University.Domain.Exceptions;

/// <summary>
/// Represents a student entity in the thesis collaboration portal.
/// Encapsulates student information and business logic for thesis management.
/// </summary>
public class Student : BaseEntity
{
    /// <summary>
    /// Gets the user ID (foreign key to AspNetUser).
    /// </summary>
    public string UserId { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the student's specialization or field of study.
    /// </summary>
    public string Specialization { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the date when the student enrolled.
    /// </summary>
    public DateTime EnrollmentDate { get; internal set; }

    /// <summary>
    /// Gets the ID of the assigned supervisor professor (nullable).
    /// </summary>
    public int? SupervisorId { get; internal set; }

    /// <summary>
    /// Gets the navigation property to the supervisor professor.
    /// </summary>
    public virtual Professor? Supervisor { get; internal set; }

    /// <summary>
    /// Gets the collection of thesis updates submitted by this student.
    /// </summary>
    public virtual ICollection<ThesisUpdate> ThesisUpdates { get; private set; } = new List<ThesisUpdate>();

    /// <summary>
    /// Creates a new student instance with the specified user ID and specialization.
    /// </summary>
    /// <param name="userId">The user ID (foreign key to AspNetUser).</param>
    /// <param name="specialization">The student's specialization.</param>
    /// <returns>A new Student instance.</returns>
    /// <exception cref="DomainException">Thrown when userId or specialization is null or empty.</exception>
    public static Student Create(string userId, string specialization)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId cannot be null or empty.");

        if (string.IsNullOrWhiteSpace(specialization))
            throw new DomainException("Specialization cannot be null or empty.");

        return new Student
        {
            UserId = userId,
            Specialization = specialization,
            EnrollmentDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Assigns a supervisor to this student.
    /// </summary>
    /// <param name="professorId">The ID of the professor to assign as supervisor.</param>
    /// <exception cref="DomainException">Thrown when the student already has a supervisor or professorId is invalid.</exception>
    public void AssignSupervisor(int professorId)
    {
        if (professorId <= 0)
            throw new DomainException("ProfessorId must be greater than zero.");

        if (SupervisorId.HasValue)
            throw new DomainException("Student already has a supervisor assigned. Remove the existing assignment first.");

        SupervisorId = professorId;
    }

    /// <summary>
    /// Removes the current supervisor assignment.
    /// </summary>
    public void RemoveSupervisor()
    {
        SupervisorId = null;
        Supervisor = null;
    }

    /// <summary>
    /// Submits a new thesis update with the specified content.
    /// </summary>
    /// <param name="content">The content of the thesis update.</param>
    /// <returns>The created ThesisUpdate entity.</returns>
    /// <exception cref="DomainException">Thrown when content is null, empty, or exceeds 10000 characters.</exception>
    public ThesisUpdate SubmitUpdate(string content)
    {
        var update = ThesisUpdate.Create(Id, content);
        ThesisUpdates.Add(update);
        return update;
    }
}
