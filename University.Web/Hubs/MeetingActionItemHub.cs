using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace University.Web.Hubs;

public sealed class MeetingActionItemHub : Hub
{
    public Task JoinMeeting(Guid meetingId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, BuildGroupName(meetingId));
    }

    public Task LeaveMeeting(Guid meetingId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, BuildGroupName(meetingId));
    }

    internal static string BuildGroupName(Guid meetingId)
    {
        return $"meeting:{meetingId}";
    }
}