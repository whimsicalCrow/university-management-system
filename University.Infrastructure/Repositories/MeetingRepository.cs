using Microsoft.EntityFrameworkCore;
using University.Domain.Aggregates.Meetings;
using University.Domain.Repositories;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public sealed class MeetingRepository : IMeetingRepository
{
    private readonly UniversityDbContext _dbContext;

    public MeetingRepository(UniversityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Meeting?> GetByIdAsync(Guid meetingId, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Meetings
            .AsNoTracking()
            .Include(meeting => meeting.Slots)
            .FirstOrDefaultAsync(meeting => meeting.Id == meetingId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Meeting?> GetForUpdateAsync(Guid meetingId, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Meetings
            .Include(meeting => meeting.Slots)
            .FirstOrDefaultAsync(meeting => meeting.Id == meetingId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Meeting>> GetUpcomingMeetingsForParticipantAsync(
        Guid participantId,
        DateTime fromUtc,
        CancellationToken cancellationToken = default)
    {
        var meetings = await _dbContext
            .Meetings
            .AsNoTracking()
            .Include(meeting => meeting.Slots)
            .Where(meeting =>
                (meeting.StudentId == participantId || meeting.SupervisorId == participantId) &&
                meeting.Slots.Any(slot => slot.StartOn >= fromUtc))
            .OrderBy(meeting => meeting.ConfirmedOn ?? meeting.Slots.Min(slot => slot.StartOn))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return meetings;
    }

    public async Task AddAsync(Meeting meeting, CancellationToken cancellationToken = default)
    {
        await _dbContext.Meetings.AddAsync(meeting, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> HasOverlappingMeetingAsync(
        Guid supervisorId,
        DateTime startUtc,
        DateTime endUtc,
        Guid? meetingIdToExclude = null,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Meetings
            .Where(meeting => meeting.SupervisorId == supervisorId && meeting.Status != MeetingStatuses.Cancelled)
            .Where(meeting => meetingIdToExclude == null || meeting.Id != meetingIdToExclude)
            .SelectMany(meeting => meeting.Slots)
            .Where(slot => slot.Status != MeetingSlotStatuses.Declined)
            .AnyAsync(slot => slot.StartOn < endUtc && startUtc < slot.EndOn, cancellationToken)
            .ConfigureAwait(false);
    }
}