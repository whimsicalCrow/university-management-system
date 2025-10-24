using MediatR;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.ProposeMeeting;

public sealed record ProposeMeetingCommand(
    Guid ThesisProjectId,
    Guid StudentId,
    string Agenda,
    IReadOnlyCollection<ProposedMeetingSlotDto> Slots,
    string? VideoConferenceUrl) : IRequest<MeetingDto>;