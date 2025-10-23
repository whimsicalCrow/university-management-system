namespace University.Domain.Aggregates.Theses;

public sealed class ThesisUpdate
{
    private readonly List<ThesisAttachment> _attachments = new();

    public ThesisUpdate(Guid authorId, string note, DateTime occurredOn, IReadOnlyCollection<ThesisAttachment>? attachments = null)
        : this(Guid.NewGuid(), authorId, note, occurredOn, attachments)
    {
    }

    private ThesisUpdate(Guid id, Guid authorId, string note, DateTime occurredOn, IReadOnlyCollection<ThesisAttachment>? attachments)
    {
        if (authorId == Guid.Empty)
        {
            throw new ArgumentException("Author id must be provided", nameof(authorId));
        }

        if (string.IsNullOrWhiteSpace(note))
        {
            throw new ArgumentException("Note must be provided", nameof(note));
        }

    Id = id;
    AuthorId = authorId;
    Note = note;
    OccurredOn = occurredOn;

        if (attachments is { Count: > 0 })
        {
            _attachments.AddRange(attachments);
        }
    }

    private ThesisUpdate()
    {
        Note = string.Empty;
        OccurredOn = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid AuthorId { get; }

    public string Note { get; private set; }

    public DateTime OccurredOn { get; private set; }

    public IReadOnlyCollection<ThesisAttachment> Attachments => _attachments.AsReadOnly();

    public void UpdateNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            throw new ArgumentException("Note must be provided", nameof(note));
        }

        Note = note;
    }

    public void Reschedule(DateTime occurredOn)
    {
        OccurredOn = occurredOn;
    }

    public void ReplaceAttachments(IReadOnlyCollection<ThesisAttachment>? attachments)
    {
        _attachments.Clear();

        if (attachments is { Count: > 0 })
        {
            _attachments.AddRange(attachments);
        }
    }
}
