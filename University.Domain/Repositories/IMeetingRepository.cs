using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using University.Domain.Aggregates.Meetings;

namespace University.Domain.Repositories;

public interface IMeetingRepository
{
    Task<Meeting?> GetByIdAsync(Guid meetingId, CancellationToken cancellationToken = default);

    Task<Meeting?> GetForUpdateAsync(Guid meetingId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Meeting>> GetUpcomingMeetingsForParticipantAsync(
        Guid participantId,
        DateTime fromUtc,
        CancellationToken cancellationToken = default);

    Task AddAsync(Meeting meeting, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingMeetingAsync(
        Guid supervisorId,
        DateTime startUtc,
        DateTime endUtc,
        Guid? meetingIdToExclude = null,
        CancellationToken cancellationToken = default);
}