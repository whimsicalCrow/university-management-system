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
}