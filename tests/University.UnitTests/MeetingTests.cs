using System;
using System.Linq;
using University.Domain.Aggregates.Meetings;

namespace University.UnitTests;

public class MeetingTests
{
    [Fact]
    public void Constructor_Throws_WhenNoSlotsProvided()
    {
        var thesisId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();

        var act = () => new Meeting(
            thesisId,
            studentId,
            supervisorId,
            "Discuss progress",
            Array.Empty<MeetingSlot>());

        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void AcceptSlot_ConfirmsMeetingAndDeclinesOthers()
    {
        var studentId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var thesisId = Guid.NewGuid();

    var firstStart = DateTime.UtcNow.AddDays(1);
    var secondStart = DateTime.UtcNow.AddDays(2);

    var slotOne = new MeetingSlot(studentId, firstStart, firstStart.AddHours(1));
    var slotTwo = new MeetingSlot(studentId, secondStart, secondStart.AddHours(1));

        var meeting = new Meeting(
            thesisId,
            studentId,
            supervisorId,
            "Sprint planning",
            new[] { slotOne, slotTwo });

        meeting.AcceptSlot(supervisorId, slotTwo.Id, "https://teams.example.com/meeting");

        Assert.True(meeting.IsConfirmed);
        Assert.Equal(MeetingStatuses.Confirmed, meeting.Status);
        Assert.Equal(slotTwo.Id, meeting.ConfirmedSlotId);
        Assert.NotNull(meeting.ConfirmedOn);
        Assert.Equal(MeetingSlotStatuses.Accepted, meeting.Slots.Single(slot => slot.Id == slotTwo.Id).Status);
        Assert.All(
            meeting.Slots.Where(slot => slot.Id != slotTwo.Id),
            slot => Assert.Equal(MeetingSlotStatuses.Declined, slot.Status));
    }

    [Fact]
    public void SuggestAlternativeSlot_AddsSlotFromSupervisor()
    {
        var studentId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var thesisId = Guid.NewGuid();

        var initialStart = DateTime.UtcNow.AddDays(1);

        var meeting = new Meeting(
            thesisId,
            studentId,
            supervisorId,
            "Discuss prototype",
            new[]
            {
                new MeetingSlot(studentId, initialStart, initialStart.AddHours(1)),
            });

        var alternativeStart = DateTime.UtcNow.AddDays(3);

        var suggested = meeting.SuggestAlternativeSlot(
            supervisorId,
            alternativeStart,
            alternativeStart.AddHours(1),
            "Prefer afternoon");

        Assert.Contains(meeting.Slots, slot => slot.Id == suggested.Id);
        Assert.Equal(MeetingSlotStatuses.Proposed, suggested.Status);
        Assert.Equal(supervisorId, suggested.ProposedById);
    }

    [Fact]
    public void AddActionItem_AddsPendingItemForParticipant()
    {
        var studentId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var thesisId = Guid.NewGuid();

        var meeting = new Meeting(
            thesisId,
            studentId,
            supervisorId,
            "Sprint sync",
            new[]
            {
                new MeetingSlot(studentId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1)),
            });

        var actionItem = meeting.AddActionItem(studentId, supervisorId, "Prepare architecture diagram", DateTime.UtcNow.AddDays(3));

        Assert.Single(meeting.ActionItems);
        Assert.Equal(actionItem.Id, meeting.ActionItems.Single().Id);
        Assert.Equal(MeetingActionItemStatuses.Pending, actionItem.Status);
        Assert.Equal(supervisorId, actionItem.OwnerId);
    }

    [Fact]
    public void UpdateActionItem_AllowsChangingOwnerDescriptionAndDueDate()
    {
        var studentId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var thesisId = Guid.NewGuid();

        var meeting = new Meeting(
            thesisId,
            studentId,
            supervisorId,
            "Sprint sync",
            new[]
            {
                new MeetingSlot(studentId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1)),
            });

        var actionItem = meeting.AddActionItem(supervisorId, studentId, "Review notes", DateTime.UtcNow.AddDays(2));

        var newDueDate = DateTime.UtcNow.AddDays(4);
        meeting.UpdateActionItem(studentId, actionItem.Id, supervisorId, "Prepare retrospective summary", newDueDate);

        var stored = meeting.ActionItems.Single();
        Assert.Equal(supervisorId, stored.OwnerId);
        Assert.Equal("Prepare retrospective summary", stored.Description);
        Assert.Equal(newDueDate, stored.DueOnUtc);
    }

    [Fact]
    public void CompleteActionItem_MarksItemCompletedAndAllowsReopen()
    {
        var studentId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var thesisId = Guid.NewGuid();

        var meeting = new Meeting(
            thesisId,
            studentId,
            supervisorId,
            "Sprint sync",
            new[]
            {
                new MeetingSlot(studentId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1)),
            });

        var actionItem = meeting.AddActionItem(studentId, supervisorId, "Share dataset");

        meeting.CompleteActionItem(supervisorId, actionItem.Id);

        var completed = meeting.ActionItems.Single();
        Assert.Equal(MeetingActionItemStatuses.Completed, completed.Status);
        Assert.NotNull(completed.CompletedOnUtc);

        meeting.ReopenActionItem(studentId, actionItem.Id);

        var reopened = meeting.ActionItems.Single();
        Assert.Equal(MeetingActionItemStatuses.Pending, reopened.Status);
        Assert.Null(reopened.CompletedOnUtc);
    }
}