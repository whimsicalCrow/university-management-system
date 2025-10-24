using System;

namespace University.Domain.Aggregates.Meetings;

public sealed class MeetingActionItem
{
    public MeetingActionItem(
        Guid createdById,
        Guid ownerId,
        string description,
        DateTime? dueOnUtc = null)
        : this(Guid.NewGuid(), createdById, ownerId, description, dueOnUtc, MeetingActionItemStatuses.Pending, DateTime.UtcNow, DateTime.UtcNow, null)
    {
    }

    private MeetingActionItem(
        Guid id,
        Guid createdById,
        Guid ownerId,
        string description,
        DateTime? dueOnUtc,
        string status,
        DateTime createdOnUtc,
        DateTime lastUpdatedOnUtc,
        DateTime? completedOnUtc)
    {
        if (createdById == Guid.Empty)
        {
            throw new ArgumentException("Creator id must be provided.", nameof(createdById));
        }

        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner id must be provided.", nameof(ownerId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must be provided.", nameof(description));
        }

        if (!MeetingActionItemStatuses.IsValid(status))
        {
            throw new ArgumentException("Unknown action item status.", nameof(status));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        CreatedById = createdById;
        OwnerId = ownerId;
        Description = description;
        DueOnUtc = dueOnUtc;
        Status = status;
        CreatedOnUtc = createdOnUtc;
        LastUpdatedOnUtc = lastUpdatedOnUtc;
        CompletedOnUtc = completedOnUtc;
    }

    private MeetingActionItem()
    {
        Description = string.Empty;
        Status = MeetingActionItemStatuses.Pending;
        CreatedOnUtc = DateTime.UtcNow;
        LastUpdatedOnUtc = CreatedOnUtc;
    }

    public Guid Id { get; private set; }

    public Guid CreatedById { get; private set; }

    public Guid OwnerId { get; private set; }

    public string Description { get; private set; }

    public DateTime? DueOnUtc { get; private set; }

    public string Status { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime LastUpdatedOnUtc { get; private set; }

    public DateTime? CompletedOnUtc { get; private set; }

    public bool IsCompleted => Status == MeetingActionItemStatuses.Completed;

    public MeetingActionItem Update(Guid ownerId, string description, DateTime? dueOnUtc)
    {
        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner id must be provided.", nameof(ownerId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must be provided.", nameof(description));
        }

        OwnerId = ownerId;
        Description = description;
        DueOnUtc = dueOnUtc;
        Touch();
        return this;
    }

    public MeetingActionItem Complete()
    {
        if (IsCompleted)
        {
            return this;
        }

        Status = MeetingActionItemStatuses.Completed;
        CompletedOnUtc = DateTime.UtcNow;
        Touch();
        return this;
    }

    public MeetingActionItem Reopen()
    {
        if (!IsCompleted)
        {
            return this;
        }

        Status = MeetingActionItemStatuses.Pending;
        CompletedOnUtc = null;
        Touch();
        return this;
    }

    private void Touch()
    {
        LastUpdatedOnUtc = DateTime.UtcNow;
    }
}