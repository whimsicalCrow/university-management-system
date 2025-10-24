using MediatR;
using University.Application.Meetings.Shared;
using University.Domain.Repositories;

namespace University.Application.Meetings.Commands.DeclineMeetingSlot;

public sealed class DeclineMeetingSlotCommandHandler : IRequestHandler<DeclineMeetingSlotCommand, MeetingDto>
{
    private readonly IMeetingRepository _meetingRepository;

    public DeclineMeetingSlotCommandHandler(IMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public async Task<MeetingDto> Handle(DeclineMeetingSlotCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _meetingRepository.GetForUpdateAsync(request.MeetingId, cancellationToken)
            .ConfigureAwait(false);

        if (meeting is null)
        {
            throw new InvalidOperationException("The meeting could not be found.");
        }

        var alternative = request.AlternativeSlot;

        if (alternative is not null)
        {
            var hasOverlap = await _meetingRepository
                .HasOverlappingMeetingAsync(meeting.SupervisorId, alternative.StartOn, alternative.EndOn, meeting.Id, cancellationToken)
                .ConfigureAwait(false);

            if (hasOverlap)
            {
                throw new InvalidOperationException("The suggested alternative overlaps with another meeting for the supervisor.");
            }
        }

        meeting.DeclineSlot(request.ProfessorId, request.SlotId, request.ResponseNote);

        if (alternative is not null)
        {
            meeting.SuggestAlternativeSlot(
                request.ProfessorId,
                alternative.StartOn,
                alternative.EndOn,
                alternative.Note);
        }

        await _meetingRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return meeting.ToMeetingDto();
    }
}
