namespace University.Web.Services;

using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Infrastructure.Data;

public interface IThesisTopicAssignmentService
{
    Task<IReadOnlyDictionary<Guid, PersistedTopicAssignment>> GetPersistedAssignmentsAsync(CancellationToken cancellationToken = default);

    Task<AssignTopicResult> AssignTopicAsync(
        Guid topicId,
        string topicTitle,
        string studentEmail,
        string professorEmail,
        CancellationToken cancellationToken = default);

    /// <summary>Returns true when the student has an active <see cref="ThesisTopicAssignment"/>.</summary>
    Task<bool> HasActiveAssignmentAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the assignment for the given topic, clears the student's supervisor,
    /// and deletes the supervision <see cref="Assignment"/> row.
    /// </summary>
    Task<RemoveAssignmentResult> RemoveAssignmentAsync(Guid topicId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all ongoing thesis assignments supervised by the given professor,
    /// including the student's domain ID and email for timeline navigation.
    /// </summary>
    Task<IReadOnlyList<ProfessorThesisView>> GetProfessorThesesAsync(string professorEmail, CancellationToken cancellationToken = default);

    /// <summary>Returns the email addresses of all professors registered in the database.</summary>
    Task<IReadOnlyList<string>> GetAllProfessorEmailsAsync(CancellationToken cancellationToken = default);
}

public sealed record PersistedTopicAssignment(
    Guid TopicId,
    string StudentEmail,
    string ProfessorEmail,
    DateTime AssignedOn);

public sealed record AssignTopicResult(bool Success, string? Error = null);
public sealed record RemoveAssignmentResult(bool Success, string? Error = null);
public sealed record ProfessorThesisView(Guid TopicId, string TopicTitle, int StudentId, string StudentEmail, DateTime AssignedOn);

public class ThesisTopicAssignmentService : IThesisTopicAssignmentService
{
    private readonly UniversityDbContext _db;

    public ThesisTopicAssignmentService(UniversityDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyDictionary<Guid, PersistedTopicAssignment>> GetPersistedAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.ThesisTopicAssignments
            .AsNoTracking()
            .Join(_db.Students.AsNoTracking(),
                assignment => assignment.StudentId,
                student => student.Id,
                (assignment, student) => new { assignment, student })
            .Join(_db.Professors.AsNoTracking(),
                left => left.assignment.ProfessorId,
                professor => professor.Id,
                (left, professor) => new { left.assignment, left.student, professor })
            .Join(_db.Users.AsNoTracking(),
                left => left.student.UserId,
                user => user.Id,
                (left, studentUser) => new { left.assignment, left.professor, studentUser })
            .Join(_db.Users.AsNoTracking(),
                left => left.professor.UserId,
                user => user.Id,
                (left, professorUser) => new PersistedTopicAssignment(
                    left.assignment.TopicId,
                    left.studentUser.Email ?? string.Empty,
                    professorUser.Email ?? string.Empty,
                    left.assignment.AssignedOn))
            .ToListAsync(cancellationToken);

        return rows
            .Where(r => r.TopicId != Guid.Empty)
            .ToDictionary(r => r.TopicId, r => r);
    }

    public async Task<AssignTopicResult> AssignTopicAsync(
        Guid topicId,
        string topicTitle,
        string studentEmail,
        string professorEmail,
        CancellationToken cancellationToken = default)
    {
        if (topicId == Guid.Empty)
        {
            return new AssignTopicResult(false, "Invalid topic id.");
        }

        if (string.IsNullOrWhiteSpace(studentEmail) || string.IsNullOrWhiteSpace(professorEmail))
        {
            return new AssignTopicResult(false, "Student and professor accounts are required.");
        }

        var studentNormalizedEmail = studentEmail.Trim().ToUpperInvariant();
        var professorNormalizedEmail = professorEmail.Trim().ToUpperInvariant();

        var studentIdentity = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == studentNormalizedEmail, cancellationToken);
        if (studentIdentity is null)
        {
            return new AssignTopicResult(false, "Student account was not found.");
        }

        var professorIdentity = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.NormalizedEmail == professorNormalizedEmail, cancellationToken);
        if (professorIdentity is null)
        {
            return new AssignTopicResult(false, "Professor account was not found.");
        }

        var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == studentIdentity.Id, cancellationToken);
        if (student is null)
        {
            return new AssignTopicResult(false, "Student domain profile was not found.");
        }

        var professor = await _db.Professors.FirstOrDefaultAsync(p => p.UserId == professorIdentity.Id, cancellationToken);
        if (professor is null)
        {
            return new AssignTopicResult(false, "Professor domain profile was not found.");
        }

        var existing = await _db.ThesisTopicAssignments.FirstOrDefaultAsync(a => a.TopicId == topicId, cancellationToken);
        if (existing is not null && existing.StudentId != student.Id)
        {
            return new AssignTopicResult(false, "This topic is already assigned to another student.");
        }

        // Enforce one-thesis-per-student: the student must not be active on a different topic.
        var studentHasOtherAssignment = await _db.ThesisTopicAssignments
            .AnyAsync(a => a.StudentId == student.Id && a.TopicId != topicId, cancellationToken);
        if (studentHasOtherAssignment)
        {
            return new AssignTopicResult(false, "This student is already assigned to another thesis. Remove the existing assignment first.");
        }

        if (existing is null)
        {
            _db.ThesisTopicAssignments.Add(ThesisTopicAssignment.Create(topicId, topicTitle, student.Id, professor.Id));
        }
        else
        {
            existing.Refresh(topicTitle, student.Id, professor.Id);
        }

        if (student.SupervisorId != professor.Id)
        {
            if (student.SupervisorId.HasValue)
            {
                student.RemoveSupervisor();
            }

            student.AssignSupervisor(professor.Id);
        }

        var hasAssignment = await _db.Assignments.AnyAsync(
            a => a.StudentId == student.Id && a.ProfessorId == professor.Id,
            cancellationToken);
        if (!hasAssignment)
        {
            _db.Assignments.Add(Assignment.Create(student.Id, professor.Id));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new AssignTopicResult(true);
    }

    public async Task<bool> HasActiveAssignmentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _db.ThesisTopicAssignments
            .AsNoTracking()
            .AnyAsync(a => a.StudentId == studentId, cancellationToken);
    }

    public async Task<RemoveAssignmentResult> RemoveAssignmentAsync(Guid topicId, CancellationToken cancellationToken = default)
    {
        if (topicId == Guid.Empty)
            return new RemoveAssignmentResult(false, "Invalid topic id.");

        var topicAssignment = await _db.ThesisTopicAssignments
            .FirstOrDefaultAsync(a => a.TopicId == topicId, cancellationToken);

        if (topicAssignment is null)
            return new RemoveAssignmentResult(false, "No assignment found for this topic.");

        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == topicAssignment.StudentId, cancellationToken);

        if (student is not null && student.SupervisorId == topicAssignment.ProfessorId)
        {
            student.RemoveSupervisor();
        }

        var supervision = await _db.Assignments
            .FirstOrDefaultAsync(
                a => a.StudentId == topicAssignment.StudentId && a.ProfessorId == topicAssignment.ProfessorId,
                cancellationToken);

        if (supervision is not null)
            _db.Assignments.Remove(supervision);

        _db.ThesisTopicAssignments.Remove(topicAssignment);
        await _db.SaveChangesAsync(cancellationToken);
        return new RemoveAssignmentResult(true);
    }

    public async Task<IReadOnlyList<ProfessorThesisView>> GetProfessorThesesAsync(
        string professorEmail,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(professorEmail))
            return Array.Empty<ProfessorThesisView>();

        var normalizedEmail = professorEmail.Trim().ToUpperInvariant();

        var userId = await _db.Users
            .AsNoTracking()
            .Where(u => u.NormalizedEmail == normalizedEmail || u.NormalizedUserName == normalizedEmail)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId is null) return Array.Empty<ProfessorThesisView>();

        var professorId = await _db.Professors
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => (int?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (professorId is null) return Array.Empty<ProfessorThesisView>();

        var rows = await _db.ThesisTopicAssignments
            .AsNoTracking()
            .Where(a => a.ProfessorId == professorId.Value)
            .Join(_db.Students.AsNoTracking(),
                a => a.StudentId,
                s => s.Id,
                (a, s) => new { a, s })
            .Join(_db.Users.AsNoTracking(),
                x => x.s.UserId,
                u => u.Id,
                (x, u) => new {
                    x.a.TopicId,
                    x.a.TopicTitle,
                    x.a.StudentId,
                    Email = u.Email ?? string.Empty,
                    x.a.AssignedOn
                })
            .OrderBy(x => x.TopicTitle)
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => new ProfessorThesisView(x.TopicId, x.TopicTitle, x.StudentId, x.Email, x.AssignedOn))
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetAllProfessorEmailsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Professors
            .AsNoTracking()
            .Join(_db.Users.AsNoTracking(),
                p => p.UserId,
                u => u.Id,
                (p, u) => u.Email ?? u.UserName ?? string.Empty)
            .Where(e => !string.IsNullOrEmpty(e))
            .OrderBy(e => e)
            .ToListAsync(cancellationToken);
    }
}