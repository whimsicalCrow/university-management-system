using System.Linq;
using MediatR;
using University.Application.Meetings.Shared;
using University.Domain.Repositories;

namespace University.Application.Meetings.Commands.AcceptMeetingSlot;

public sealed class AcceptMeetingSlotCommandHandler : IRequestHandler<AcceptMeetingSlotCommand, MeetingDto>
{
    private readonly IMeetingRepository _meetingRepository;

    public AcceptMeetingSlotCommandHandler(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public async Task<MeetingDto> Handle(AcceptMeetingSlotCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _meetingRepository.GetForUpdateAsync(request.MeetingId, cancellationToken)
            .ConfigureAwait(false);

        if (meeting is null)
        {
            throw new InvalidOperationException("The meeting could not be found.");
        }

        var slot = meeting.Slots.FirstOrDefault(slot => slot.Id == request.SlotId);

        if (slot is null)
        {
            throw new InvalidOperationException("The requested meeting slot does not exist.");
        }

        var hasOverlap = await _meetingRepository
            .HasOverlappingMeetingAsync(meeting.SupervisorId, slot.StartOn, slot.EndOn, meeting.Id, cancellationToken)
            .ConfigureAwait(false);

        if (hasOverlap)
        {
            throw new InvalidOperationException("The supervisor is already booked for another meeting during the selected slot.");
        }

        meeting.AcceptSlot(request.ProfessorId, request.SlotId, request.VideoConferenceUrl);

        await _meetingRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return meeting.ToMeetingDto();
    }
}