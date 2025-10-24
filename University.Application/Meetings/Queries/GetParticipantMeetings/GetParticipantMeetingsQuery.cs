using MediatR;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Queries.GetParticipantMeetings;

public sealed record GetParticipantMeetingsQuery(Guid ParticipantId) : IRequest<IReadOnlyCollection<MeetingDto>>;