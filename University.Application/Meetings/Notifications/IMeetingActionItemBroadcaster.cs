using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Notifications;

public interface IMeetingActionItemBroadcaster
{
    Task ItemAddedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken);

    Task ItemUpdatedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken);

    Task ItemStatusChangedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken);
}