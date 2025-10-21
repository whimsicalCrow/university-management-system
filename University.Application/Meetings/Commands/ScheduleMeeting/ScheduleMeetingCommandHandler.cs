using MediatR;
using University.Application.DTOs;
using University.Domain.Entities;
using University.Domain.Interfaces;

namespace University.Application.Meetings.Commands.ScheduleMeeting;

public class ScheduleMeetingCommandHandler : IRequestHandler<ScheduleMeetingCommand, MeetingDto>
{
    private readonly IThesisProjectRepository _thesisProjectRepository;
    private readonly IMeetingRepository _meetingRepository;

    public ScheduleMeetingCommandHandler(
        IThesisProjectRepository thesisProjectRepository,
        IMeetingRepository meetingRepository)
    {
        _thesisProjectRepository = thesisProjectRepository;
        _meetingRepository = meetingRepository;
    }

    public async Task<MeetingDto> Handle(ScheduleMeetingCommand request, CancellationToken cancellationToken)
    {
        var project = await _thesisProjectRepository.GetByIdAsync(request.ThesisProjectId, cancellationToken)
            ?? throw new InvalidOperationException($"Thesis project {request.ThesisProjectId} not found.");

        var meeting = new Meeting
        {
            ThesisProjectId = project.Id,
            ScheduledForUtc = request.ScheduledForUtc,
            Duration = request.Duration,
            Location = request.Location,
            Agenda = request.Agenda,
            Status = MeetingStatus.Confirmed
        };

        meeting = await _meetingRepository.AddAsync(meeting, cancellationToken);

        return new MeetingDto(
            meeting.Id,
            meeting.ThesisProjectId,
            meeting.ScheduledForUtc,
            meeting.Duration,
            meeting.Location,
            meeting.Agenda,
            meeting.Notes,
            meeting.Status.ToString()
        );
    }
}