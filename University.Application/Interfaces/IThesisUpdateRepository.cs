namespace University.Application.Interfaces;

using University.Domain.Entities;

/// <summary>
/// Repository contract for <see cref="ThesisUpdate"/> and related professor-review operations.
/// Implemented in the Infrastructure layer; consumed by Application command handlers.
/// </summary>
public interface IThesisUpdateRepository
{
    /// <summary>
    /// Returns the <see cref="ThesisUpdate"/> with the given primary key (including its
    /// navigation to the owning <see cref="Student"/>), or null if not found.
    /// </summary>
    Task<ThesisUpdate?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Returns the <see cref="Professor"/> whose Identity account email/username matches
    /// <paramref name="email"/>, or null when no professor profile exists for that user.
    /// Resolves email → Identity UserId → Professor in a single logical lookup.
    /// </summary>
    Task<Professor?> FindProfessorByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Returns true when <paramref name="professorId"/> has an active assignment that includes
    /// <paramref name="studentId"/> — used to authorise professor reviews.
    /// </summary>
    Task<bool> IsProfessorAssignedToStudentAsync(int professorId, int studentId, CancellationToken ct = default);

    /// <summary>
    /// Persists a <see cref="Feedback"/> record and flushes changes.
    /// Returns the new feedback primary key.
    /// </summary>
    Task<int> AddFeedbackAsync(Feedback feedback, CancellationToken ct = default);

    /// <summary>Persists any pending changes on tracked entities.</summary>
    Task SaveChangesAsync(CancellationToken ct = default);
}
