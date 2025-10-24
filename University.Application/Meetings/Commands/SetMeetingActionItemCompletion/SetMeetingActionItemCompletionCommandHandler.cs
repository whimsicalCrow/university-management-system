using System.Linq;
using MediatR;
using University.Application.Meetings.Notifications;
using University.Application.Meetings.Shared;
using University.Domain.Repositories;

namespace University.Application.Meetings.Commands.SetMeetingActionItemCompletion;

public sealed class SetMeetingActionItemCompletionCommandHandler : IRequestHandler<SetMeetingActionItemCompletionCommand, MeetingDto>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IMeetingActionItemBroadcaster _broadcaster;

    public SetMeetingActionItemCompletionCommandHandler(
        IMeetingRepository meetingRepository,
        IMeetingActionItemBroadcaster broadcaster)
    {
        _meetingRepository = meetingRepository;
        _broadcaster = broadcaster;
    }

    public async Task<MeetingDto> Handle(SetMeetingActionItemCompletionCommand request, CancellationToken cancellationToken)
    {
        var meeting = await _meetingRepository.GetForUpdateAsync(request.MeetingId, cancellationToken)
            .ConfigureAwait(false);

        if (meeting is null)
        {
            throw new InvalidOperationException("The meeting could not be found.");
        }

        if (request.IsCompleted)
        {
            meeting.CompleteActionItem(request.ActorId, request.ActionItemId);
        }
        else
        {
            meeting.ReopenActionItem(request.ActorId, request.ActionItemId);
        }

        await _meetingRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var meetingDto = meeting.ToMeetingDto();
        var actionItemDto = meetingDto.ActionItems.Single(item => item.Id == request.ActionItemId);

        await _broadcaster.ItemStatusChangedAsync(meetingDto, actionItemDto, cancellationToken).ConfigureAwait(false);

        return meetingDto;
    }
}