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
}

public sealed record PersistedTopicAssignment(
    Guid TopicId,
    string StudentEmail,
    string ProfessorEmail,
    DateTime AssignedOn);

public sealed record AssignTopicResult(bool Success, string? Error = null);

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
}