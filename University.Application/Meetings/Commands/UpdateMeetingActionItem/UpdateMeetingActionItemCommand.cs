using MediatR;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.UpdateMeetingActionItem;

public sealed record UpdateMeetingActionItemCommand(
    Guid MeetingId,
    Guid ActionItemId,
    Guid ActorId,
    Guid OwnerId,
    string Description,
    DateTime? DueOnUtc) : IRequest<MeetingDto>;