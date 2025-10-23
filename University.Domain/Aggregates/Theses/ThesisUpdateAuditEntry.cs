using System;

namespace University.Domain.Aggregates.Theses;

public sealed class ThesisUpdateAuditEntry
{
    public ThesisUpdateAuditEntry(
        Guid actorId,
        string action,
        string? details = null,
        string? fromStatus = null,
        string? toStatus = null)
        : this(Guid.NewGuid(), actorId, action, DateTime.UtcNow, details, fromStatus, toStatus)
    {
    }

    private ThesisUpdateAuditEntry(
        Guid id,
        Guid actorId,
        string action,
        DateTime occurredOn,
        string? details,
        string? fromStatus,
        string? toStatus)
    {
        if (actorId == Guid.Empty)
        {
            throw new ArgumentException("Actor id must be provided", nameof(actorId));
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action must be provided", nameof(action));
        }

        Id = id;
        ActorId = actorId;
        Action = action;
        OccurredOn = occurredOn;
        Details = details;
        FromStatus = fromStatus;
        ToStatus = toStatus;
    }

    private ThesisUpdateAuditEntry()
    {
        Action = string.Empty;
    }

    public Guid Id { get; private set; }

    public Guid ActorId { get; private set; }

    public string Action { get; private set; }

    public DateTime OccurredOn { get; private set; }

    public string? Details { get; private set; }

    public string? FromStatus { get; private set; }

    public string? ToStatus { get; private set; }
}