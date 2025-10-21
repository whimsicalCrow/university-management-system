using System.Linq;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Domain.Interfaces;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public class ThesisProjectRepository : IThesisProjectRepository
{
    private readonly UniversityDbContext _db;

    public ThesisProjectRepository(UniversityDbContext db) => _db = db;

    public async Task<ThesisProject> AddAsync(ThesisProject project, CancellationToken cancellationToken = default)
    {
        _db.ThesisProjects.Add(project);
        await _db.SaveChangesAsync(cancellationToken);
        return project;
    }

    public Task<ThesisProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.ThesisProjects
            .Include(p => p.Updates)
            .Include(p => p.Meetings)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ThesisProject>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default)
    {
        return await _db.ThesisProjects
            .Where(p => p.ProfessorId == professorId)
            .OrderByDescending(p => p.LastUpdatedAtUtc ?? p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ThesisProject>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _db.ThesisProjects
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.LastUpdatedAtUtc ?? p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ThesisProject project, CancellationToken cancellationToken = default)
    {
        _db.ThesisProjects.Update(project);
        await _db.SaveChangesAsync(cancellationToken);
    }
}