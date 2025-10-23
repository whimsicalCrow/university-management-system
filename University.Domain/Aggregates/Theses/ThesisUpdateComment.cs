using System;

namespace University.Domain.Aggregates.Theses;

public sealed class ThesisUpdateComment
{
    public ThesisUpdateComment(Guid authorId, string content, Guid? parentCommentId = null)
        : this(Guid.NewGuid(), authorId, content, DateTime.UtcNow, parentCommentId)
    {
    }

    private ThesisUpdateComment(Guid id, Guid authorId, string content, DateTime createdOn, Guid? parentCommentId)
    {
        if (authorId == Guid.Empty)
        {
            throw new ArgumentException("Author id must be provided", nameof(authorId));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content must be provided", nameof(content));
        }

        Id = id;
        AuthorId = authorId;
        Content = content;
        CreatedOn = createdOn;
        ParentCommentId = parentCommentId;
    }

    private ThesisUpdateComment()
    {
        Content = string.Empty;
    }

    public Guid Id { get; private set; }

    public Guid AuthorId { get; private set; }

    public string Content { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public DateTime? LastEditedOn { get; private set; }

    public Guid? ParentCommentId { get; private set; }

    public void Edit(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content must be provided", nameof(content));
        }

        Content = content;
        LastEditedOn = DateTime.UtcNow;
    }
}