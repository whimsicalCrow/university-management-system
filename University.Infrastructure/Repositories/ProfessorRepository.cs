using System.Linq;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Domain.Interfaces;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly UniversityDbContext _db;

    public ProfessorRepository(UniversityDbContext db) => _db = db;

    public async Task<Professor> AddAsync(Professor professor, CancellationToken cancellationToken = default)
    {
        _db.Professors.Add(professor);
        await _db.SaveChangesAsync(cancellationToken);
        return professor;
    }

    public Task<Professor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _db.Professors.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Professor>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Professors.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToListAsync(cancellationToken);
    }
}