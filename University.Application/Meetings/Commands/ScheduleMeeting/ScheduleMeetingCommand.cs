using MediatR;
using University.Application.DTOs;

namespace University.Application.Meetings.Commands.ScheduleMeeting;

public record ScheduleMeetingCommand(
    Guid ThesisProjectId,
    DateTime ScheduledForUtc,
    TimeSpan Duration,
    string Location,
    string Agenda
) : IRequest<MeetingDto>;