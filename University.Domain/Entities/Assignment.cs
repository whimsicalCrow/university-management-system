namespace University.Domain.Entities;

using University.Domain.Exceptions;

/// <summary>
/// Represents an assignment relationship between a student and a supervising professor.
/// Tracks when a professor is assigned to guide a student's thesis.
/// </summary>
public class Assignment : BaseEntity
{
    /// <summary>
    /// Gets the ID of the student being assigned (foreign key to Student).
    /// </summary>
    public int StudentId { get; internal set; }

    /// <summary>
    /// Gets the ID of the professor assigned as supervisor (foreign key to Professor).
    /// </summary>
    public int ProfessorId { get; internal set; }

    /// <summary>
    /// Gets the date when the assignment was created.
    /// </summary>
    public DateTime AssignedDate { get; internal set; }

    /// <summary>
    /// Gets the navigation property to the assigned student.
    /// </summary>
    public virtual Student? Student { get; private set; }

    /// <summary>
    /// Gets the navigation property to the assigned professor.
    /// </summary>
    public virtual Professor? Professor { get; private set; }

    /// <summary>
    /// Creates a new assignment between a student and a professor.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="professorId">The ID of the professor.</param>
    /// <returns>A new Assignment instance.</returns>
    /// <exception cref="DomainException">Thrown when studentId or professorId is invalid.</exception>
    public static Assignment Create(int studentId, int professorId)
    {
        if (studentId <= 0)
            throw new DomainException("StudentId must be greater than zero.");

        if (professorId <= 0)
            throw new DomainException("ProfessorId must be greater than zero.");

        return new Assignment
        {
            StudentId = studentId,
            ProfessorId = professorId,
            AssignedDate = DateTime.UtcNow
        };
    }
}
