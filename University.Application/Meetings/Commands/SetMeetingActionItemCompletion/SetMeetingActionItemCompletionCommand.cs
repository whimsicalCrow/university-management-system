using MediatR;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.SetMeetingActionItemCompletion;

public sealed record SetMeetingActionItemCompletionCommand(
    Guid MeetingId,
    Guid ActionItemId,
    Guid ActorId,
    bool IsCompleted) : IRequest<MeetingDto>;