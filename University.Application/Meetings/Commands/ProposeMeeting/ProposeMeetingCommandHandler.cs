using MediatR;
using University.Application.Meetings.Shared;
using University.Domain.Aggregates.Meetings;
using University.Domain.Repositories;

namespace University.Application.Meetings.Commands.ProposeMeeting;

public sealed class ProposeMeetingCommandHandler : IRequestHandler<ProposeMeetingCommand, MeetingDto>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IThesisProjectRepository _thesisRepository;

    public ProposeMeetingCommandHandler(
        IMeetingRepository meetingRepository,
        IThesisProjectRepository thesisRepository)
    {
        _meetingRepository = meetingRepository;
        _thesisRepository = thesisRepository;
    }

    public async Task<MeetingDto> Handle(ProposeMeetingCommand request, CancellationToken cancellationToken)
    {
        var thesis = await _thesisRepository.GetByIdAsync(request.ThesisProjectId, cancellationToken)
            .ConfigureAwait(false);

        if (thesis is null)
        {
            throw new InvalidOperationException("The thesis project could not be found.");
        }

        if (thesis.StudentId != request.StudentId)
        {
            throw new UnauthorizedAccessException("Only the thesis student can propose meetings.");
        }

        foreach (var slot in request.Slots)
        {
            var hasOverlap = await _meetingRepository
                .HasOverlappingMeetingAsync(thesis.SupervisorId, slot.StartOn, slot.EndOn, null, cancellationToken)
                .ConfigureAwait(false);

            if (hasOverlap)
            {
                throw new InvalidOperationException("The supervisor is already booked for one of the proposed slots.");
            }
        }

        var slotEntities = request.Slots
            .Select(slot => new MeetingSlot(request.StudentId, slot.StartOn, slot.EndOn, MeetingSlotStatuses.Proposed, slot.Note))
            .ToArray();

        var meeting = new Meeting(
            request.ThesisProjectId,
            request.StudentId,
            thesis.SupervisorId,
            request.Agenda,
            slotEntities,
            request.VideoConferenceUrl);

        await _meetingRepository.AddAsync(meeting, cancellationToken).ConfigureAwait(false);

        return meeting.ToMeetingDto();
    }
}