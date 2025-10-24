using System;

namespace University.Domain.Aggregates.Meetings;

public sealed class MeetingSlot
{
    public MeetingSlot(
        Guid proposedById,
        DateTime startOn,
        DateTime endOn,
        string status = MeetingSlotStatuses.Proposed,
        string? note = null)
        : this(Guid.NewGuid(), proposedById, startOn, endOn, status, note, DateTime.UtcNow)
    {
    }

    private MeetingSlot(
        Guid id,
        Guid proposedById,
        DateTime startOn,
        DateTime endOn,
        string status,
        string? note,
        DateTime proposedOn)
    {
        if (proposedById == Guid.Empty)
        {
            throw new ArgumentException("Proposer id must be provided", nameof(proposedById));
        }

        if (startOn >= endOn)
        {
            throw new ArgumentException("Meeting slot end time must be after the start time.", nameof(endOn));
        }

        if (!MeetingSlotStatuses.IsValid(status))
        {
            throw new ArgumentException("Unknown meeting slot status.", nameof(status));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        ProposedById = proposedById;
        StartOn = startOn;
        EndOn = endOn;
        Status = status;
        Note = note;
        ProposedOn = proposedOn;
    }

    private MeetingSlot()
    {
        StartOn = DateTime.UtcNow;
        EndOn = StartOn.AddHours(1);
        Status = MeetingSlotStatuses.Proposed;
        ProposedOn = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ProposedById { get; private set; }

    public DateTime StartOn { get; private set; }

    public DateTime EndOn { get; private set; }

    public string Status { get; private set; }

    public string? Note { get; private set; }

    public string? ResponseNote { get; private set; }

    public DateTime ProposedOn { get; private set; }

    public DateTime? RespondedOn { get; private set; }

    public bool IsProposed => Status == MeetingSlotStatuses.Proposed;

    public bool IsAccepted => Status == MeetingSlotStatuses.Accepted;

    public MeetingSlot Accept()
    {
        if (!IsProposed)
        {
            throw new InvalidOperationException("Only proposed slots can be accepted.");
        }

        Status = MeetingSlotStatuses.Accepted;
        RespondedOn = DateTime.UtcNow;
        return this;
    }

    public MeetingSlot Decline(string? responseNote = null)
    {
        if (Status == MeetingSlotStatuses.Declined)
        {
            return this;
        }

        Status = MeetingSlotStatuses.Declined;
        ResponseNote = responseNote;
        RespondedOn = DateTime.UtcNow;
        return this;
    }

    public MeetingSlot UpdateNote(string? note)
    {
        Note = note;
        return this;
    }
}