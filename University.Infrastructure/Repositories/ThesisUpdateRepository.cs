namespace University.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using University.Application.Interfaces;
using University.Domain.Entities;
using University.Infrastructure.Data;

/// <summary>
/// EF Core–backed implementation of <see cref="IThesisUpdateRepository"/>.
/// </summary>
public sealed class ThesisUpdateRepository : IThesisUpdateRepository
{
    private readonly UniversityDbContext _db;

    public ThesisUpdateRepository(UniversityDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<ThesisUpdate?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.ThesisUpdates
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<Professor?> FindProfessorByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalised = email.Trim().ToUpperInvariant();

        var userId = await _db.Users
            .AsNoTracking()
            .Where(u => u.NormalizedEmail == normalised || u.NormalizedUserName == normalised)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct);

        if (userId is null) return null;

        return await _db.Professors
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);
    }

    /// <inheritdoc />
    public async Task<bool> IsProfessorAssignedToStudentAsync(
        int professorId, int studentId, CancellationToken ct = default)
    {
        // Check ThesisTopicAssignments (primary assignment table used throughout the app)
        return await _db.ThesisTopicAssignments
            .AnyAsync(a => a.ProfessorId == professorId && a.StudentId == studentId, ct);
    }

    /// <inheritdoc />
    public async Task<int> AddFeedbackAsync(Feedback feedback, CancellationToken ct = default)
    {
        _db.Feedback.Add(feedback);
        await _db.SaveChangesAsync(ct);
        return feedback.Id;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }
}
