using MediatR;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.DeclineMeetingSlot;

public sealed record DeclineMeetingSlotCommand(
    Guid MeetingId,
    Guid SlotId,
    Guid ProfessorId,
    string? ResponseNote,
    ProposedMeetingSlotDto? AlternativeSlot) : IRequest<MeetingDto>;