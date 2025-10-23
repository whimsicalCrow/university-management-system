using System;

namespace University.Domain.Aggregates.Theses;

public sealed class ThesisUpdateStatusChange
{
    public ThesisUpdateStatusChange(string status, Guid changedBy, DateTime changedOn)
        : this(Guid.NewGuid(), status, changedBy, changedOn)
    {
    }

    private ThesisUpdateStatusChange(Guid id, string status, Guid changedBy, DateTime changedOn)
    {
        if (!ThesisUpdateStatuses.IsValid(status))
        {
            throw new ArgumentException("Unknown status for thesis update", nameof(status));
        }

        if (changedBy == Guid.Empty)
        {
            throw new ArgumentException("Actor id must be provided", nameof(changedBy));
        }

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Status = status;
        ChangedBy = changedBy;
        ChangedOn = changedOn;
    }

    private ThesisUpdateStatusChange()
    {
        Status = ThesisUpdateStatuses.Draft;
        ChangedBy = Guid.Empty;
        ChangedOn = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Status { get; private set; }

    public Guid ChangedBy { get; private set; }

    public DateTime ChangedOn { get; private set; }
}