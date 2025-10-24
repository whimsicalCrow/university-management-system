using System.Linq;
using University.Domain.Aggregates.Meetings;

namespace University.Application.Meetings.Shared;

internal static class MeetingMappingExtensions
{
    public static MeetingDto ToMeetingDto(this Meeting meeting)
    {
        return new MeetingDto(
            meeting.Id,
            meeting.ThesisProjectId,
            meeting.StudentId,
            meeting.SupervisorId,
            meeting.Agenda,
            meeting.Status,
            meeting.ConfirmedSlotId,
            meeting.VideoConferenceUrl,
            meeting.CreatedOn,
            meeting.LastUpdatedOn,
            meeting.ConfirmedOn,
            meeting.Slots.Select(slot => slot.ToMeetingSlotDto()).ToArray(),
            meeting.ActionItems.Select(item => item.ToMeetingActionItemDto()).ToArray());
    }

    private static MeetingSlotDto ToMeetingSlotDto(this MeetingSlot slot)
    {
        return new MeetingSlotDto(
            slot.Id,
            slot.StartOn,
            slot.EndOn,
            slot.Status,
            slot.ProposedById,
            slot.Note,
            slot.ResponseNote,
            slot.ProposedOn,
            slot.RespondedOn);
    }

    private static MeetingActionItemDto ToMeetingActionItemDto(this MeetingActionItem item)
    {
        return new MeetingActionItemDto(
            item.Id,
            item.CreatedById,
            item.OwnerId,
            item.Description,
            item.Status,
            item.DueOnUtc,
            item.CreatedOnUtc,
            item.LastUpdatedOnUtc,
            item.CompletedOnUtc);
    }
}