using System;
using System.Collections.Generic;
using System.Linq;

namespace University.Domain.Aggregates.Theses;

public sealed class ThesisUpdate
{
    private readonly List<ThesisAttachment> _attachments = new();
    private readonly List<ThesisUpdateComment> _comments = new();
    private readonly List<ThesisUpdateAuditEntry> _auditTrail = new();

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

    public IReadOnlyCollection<ThesisUpdateComment> Comments => _comments.AsReadOnly();

    public IReadOnlyCollection<ThesisUpdateAuditEntry> AuditTrail => _auditTrail.AsReadOnly();

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

    public ThesisUpdateComment AddComment(Guid authorId, string content, Guid? parentCommentId = null)
    {
        if (parentCommentId is { } commentId && _comments.All(comment => comment.Id != commentId))
        {
            throw new InvalidOperationException("Cannot reply to a comment that does not exist on this update.");
        }

        var comment = new ThesisUpdateComment(authorId, content, parentCommentId);
        _comments.Add(comment);
        Touch();
        return comment;
    }

    public ThesisUpdateComment EditComment(Guid commentId, Guid authorId, string content)
    {
        var comment = _comments.FirstOrDefault(item => item.Id == commentId)
            ?? throw new InvalidOperationException("The comment could not be found on this update.");

        if (comment.AuthorId != authorId)
        {
            throw new InvalidOperationException("Only the original author can edit this comment.");
        }

        comment.Edit(content);
        Touch();
        return comment;
    }

    public ThesisUpdateAuditEntry AddAuditEntry(
        Guid actorId,
        string action,
        string? details = null,
        string? fromStatus = null,
        string? toStatus = null)
    {
        var auditEntry = new ThesisUpdateAuditEntry(actorId, action, details, fromStatus, toStatus);
        _auditTrail.Add(auditEntry);
        return auditEntry;
    }

    public bool TryGetComment(Guid commentId, out ThesisUpdateComment? comment)
    {
        comment = _comments.FirstOrDefault(item => item.Id == commentId);
        return comment is not null;
    }

    private void Touch()
    {
        LastModifiedOn = DateTime.UtcNow;
    }
}
