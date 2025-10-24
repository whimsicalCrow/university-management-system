using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using University.Application.Meetings.Notifications;
using University.Application.Meetings.Shared;
using University.Web.Hubs;

namespace University.Web.Services;

public sealed class SignalRMeetingActionItemBroadcaster : IMeetingActionItemBroadcaster
{
    private readonly IHubContext<MeetingActionItemHub> _hubContext;

    public SignalRMeetingActionItemBroadcaster(IHubContext<MeetingActionItemHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task ItemAddedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken)
    {
        return BroadcastAsync(meeting.Id, "ActionItemAdded", meeting, actionItem, cancellationToken);
    }

    public Task ItemUpdatedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken)
    {
        return BroadcastAsync(meeting.Id, "ActionItemUpdated", meeting, actionItem, cancellationToken);
    }

    public Task ItemStatusChangedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken)
    {
        return BroadcastAsync(meeting.Id, "ActionItemStatusChanged", meeting, actionItem, cancellationToken);
    }

    private Task BroadcastAsync(Guid meetingId, string eventName, MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken)
    {
        var payload = new MeetingActionItemChangedMessage(meetingId, eventName, actionItem);

        return _hubContext
            .Clients
            .Group(MeetingActionItemHub.BuildGroupName(meetingId))
            .SendAsync(eventName, payload, cancellationToken);
    }
}