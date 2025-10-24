using MediatR;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.AcceptMeetingSlot;

public sealed record AcceptMeetingSlotCommand(
    Guid MeetingId,
    Guid SlotId,
    Guid ProfessorId,
    string? VideoConferenceUrl) : IRequest<MeetingDto>;