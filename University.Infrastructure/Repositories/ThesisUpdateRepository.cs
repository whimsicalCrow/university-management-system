using System.Linq;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Domain.Interfaces;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public class ThesisUpdateRepository : IThesisUpdateRepository
{
    private readonly UniversityDbContext _db;

    public ThesisUpdateRepository(UniversityDbContext db) => _db = db;

    public async Task<ThesisUpdate> AddAsync(ThesisUpdate update, CancellationToken cancellationToken = default)
    {
        _db.ThesisUpdates.Add(update);
        await _db.SaveChangesAsync(cancellationToken);
        return update;
    }

    public async Task<IReadOnlyList<ThesisUpdate>> GetByThesisIdAsync(Guid thesisProjectId, CancellationToken cancellationToken = default)
    {
        return await _db.ThesisUpdates
            .Where(u => u.ThesisProjectId == thesisProjectId)
            .OrderByDescending(u => u.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}