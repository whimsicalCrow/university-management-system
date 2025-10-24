using System.Linq;
using MediatR;
using University.Application.Meetings.Notifications;
using University.Application.Meetings.Shared;
using University.Domain.Repositories;

namespace University.Application.Meetings.Commands.UpdateMeetingActionItem;

public sealed class UpdateMeetingActionItemCommandHandler : IRequestHandler<UpdateMeetingActionItemCommand, MeetingDto>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IMeetingActionItemBroadcaster _broadcaster;

    public UpdateMeetingActionItemCommandHandler(
        IMeetingRepository meetingRepository,
        IMeetingActionItemBroadcaster broadcaster)
    {
        _meetingRepository = meetingRepository;
        _broadcaster = broadcaster;
    }

    public async Task<MeetingDto> Handle(UpdateMeetingActionItemCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _meetingRepository.GetForUpdateAsync(request.MeetingId, cancellationToken)
            .ConfigureAwait(false);

        if (meeting is null)
        {
            throw new InvalidOperationException("The meeting could not be found.");
        }

        meeting.UpdateActionItem(request.ActorId, request.ActionItemId, request.OwnerId, request.Description, request.DueOnUtc);

        await _meetingRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var meetingDto = meeting.ToMeetingDto();
        var actionItemDto = meetingDto.ActionItems.Single(item => item.Id == request.ActionItemId);

        await _broadcaster.ItemUpdatedAsync(meetingDto, actionItemDto, cancellationToken).ConfigureAwait(false);

        return meetingDto;
    }
}