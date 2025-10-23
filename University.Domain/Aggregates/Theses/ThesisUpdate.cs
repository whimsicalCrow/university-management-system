namespace University.Domain.Aggregates.Theses;

public sealed class ThesisUpdate
{
    private readonly List<ThesisAttachment> _attachments = new();

    public ThesisUpdate(
        Guid authorId,
        string note,
        DateTime occurredOn,
        string status,
        IReadOnlyCollection<ThesisAttachment>? attachments = null)
        : this(Guid.NewGuid(), authorId, note, occurredOn, status, attachments)
    {
    }

    private ThesisUpdate(
        Guid id,
        Guid authorId,
        string note,
        DateTime occurredOn,
        string status,
        IReadOnlyCollection<ThesisAttachment>? attachments)
    {
        if (authorId == Guid.Empty)
        {
            throw new ArgumentException("Author id must be provided", nameof(authorId));
        }

        if (string.IsNullOrWhiteSpace(note))
        {
            throw new ArgumentException("Note must be provided", nameof(note));
        }

        if (!ThesisUpdateStatuses.IsValid(status))
        {
            throw new ArgumentException("Unknown status for thesis update", nameof(status));
        }

        Id = id;
        AuthorId = authorId;
        Note = note;
        OccurredOn = occurredOn;
        Status = status;
        LastModifiedOn = occurredOn;

        if (attachments is { Count: > 0 })
        {
            _attachments.AddRange(attachments);
        }
    }

    private ThesisUpdate()
    {
        Note = string.Empty;
        OccurredOn = DateTime.UtcNow;
        Status = ThesisUpdateStatuses.Draft;
        LastModifiedOn = OccurredOn;
    }

    public Guid Id { get; private set; }

    public Guid AuthorId { get; }

    public string Note { get; private set; }

    public DateTime OccurredOn { get; private set; }

    public string Status { get; private set; }

    public DateTime LastModifiedOn { get; private set; }

    public IReadOnlyCollection<ThesisAttachment> Attachments => _attachments.AsReadOnly();

    public void UpdateNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            throw new ArgumentException("Note must be provided", nameof(note));
        }

        Note = note;
        Touch();
    }

    public void ChangeStatus(string status)
    {
        if (!ThesisUpdateStatuses.IsValid(status))
        {
            throw new ArgumentException("Unknown status for thesis update", nameof(status));
        }

        Status = status;
        Touch();
    }

    public void Reschedule(DateTime occurredOn)
    {
        OccurredOn = occurredOn;
        Touch();
    }

    public void ReplaceAttachments(IReadOnlyCollection<ThesisAttachment>? attachments)
    {
        _attachments.Clear();

        if (attachments is { Count: > 0 })
        {
            _attachments.AddRange(attachments);
        }

        Touch();
    }

    private void Touch()
    {
        LastModifiedOn = DateTime.UtcNow;
    }
}
