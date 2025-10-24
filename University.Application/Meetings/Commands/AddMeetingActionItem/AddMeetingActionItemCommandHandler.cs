using System.Linq;
using MediatR;
using University.Application.Meetings.Notifications;
using University.Application.Meetings.Shared;
using University.Domain.Repositories;

namespace University.Application.Meetings.Commands.AddMeetingActionItem;

public sealed class AddMeetingActionItemCommandHandler : IRequestHandler<AddMeetingActionItemCommand, MeetingDto>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IMeetingActionItemBroadcaster _broadcaster;

    public AddMeetingActionItemCommandHandler(
        IMeetingRepository meetingRepository,
        IMeetingActionItemBroadcaster broadcaster)
    {
        _meetingRepository = meetingRepository;
        _broadcaster = broadcaster;
    }

    public async Task<MeetingDto> Handle(AddMeetingActionItemCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _meetingRepository.GetForUpdateAsync(request.MeetingId, cancellationToken)
            .ConfigureAwait(false);

        if (meeting is null)
        {
            throw new InvalidOperationException("The meeting could not be found.");
        }

        var actionItem = meeting.AddActionItem(request.ActorId, request.OwnerId, request.Description, request.DueOnUtc);

        await _meetingRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var meetingDto = meeting.ToMeetingDto();
        var actionItemDto = meetingDto.ActionItems.Single(item => item.Id == actionItem.Id);

        await _broadcaster.ItemAddedAsync(meetingDto, actionItemDto, cancellationToken).ConfigureAwait(false);

        return meetingDto;
    }
}