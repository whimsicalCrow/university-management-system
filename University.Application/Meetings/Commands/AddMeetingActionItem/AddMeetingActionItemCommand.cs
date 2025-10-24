using MediatR;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.AddMeetingActionItem;

public sealed record AddMeetingActionItemCommand(
    Guid MeetingId,
    Guid ActorId,
    Guid OwnerId,
    string Description,
    DateTime? DueOnUtc) : IRequest<MeetingDto>;