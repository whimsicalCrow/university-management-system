using System.Linq;
using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Domain.Interfaces;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public class MeetingRepository : IMeetingRepository
{
    private readonly UniversityDbContext _db;

    public MeetingRepository(UniversityDbContext db) => _db = db;

    public async Task<Meeting> AddAsync(Meeting meeting, CancellationToken cancellationToken = default)
    {
        _db.Meetings.Add(meeting);
        await _db.SaveChangesAsync(cancellationToken);
        return meeting;
    }

    public async Task<IReadOnlyList<Meeting>> GetByThesisIdAsync(Guid thesisProjectId, CancellationToken cancellationToken = default)
    {
        return await _db.Meetings
            .Where(m => m.ThesisProjectId == thesisProjectId)
            .OrderByDescending(m => m.ScheduledForUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Meeting meeting, CancellationToken cancellationToken = default)
    {
        _db.Meetings.Update(meeting);
        await _db.SaveChangesAsync(cancellationToken);
    }
}