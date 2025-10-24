using MediatR;
using University.Application.Meetings.Shared;
using University.Domain.Repositories;

namespace University.Application.Meetings.Queries.GetParticipantMeetings;

public sealed class GetParticipantMeetingsQueryHandler : IRequestHandler<GetParticipantMeetingsQuery, IReadOnlyCollection<MeetingDto>>
{
    private readonly IMeetingRepository _meetingRepository;

    public GetParticipantMeetingsQueryHandler(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public async Task<IReadOnlyCollection<MeetingDto>> Handle(GetParticipantMeetingsQuery request, CancellationToken cancellationToken)
    {
        var meetings = await _meetingRepository
            .GetUpcomingMeetingsForParticipantAsync(request.ParticipantId, DateTime.UtcNow, cancellationToken)
            .ConfigureAwait(false);

        return meetings.Select(meeting => meeting.ToMeetingDto()).ToArray();
    }
}