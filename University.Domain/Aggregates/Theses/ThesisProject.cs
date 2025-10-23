using System;
using System.Collections.Generic;
using System.Linq;
using University.Domain.Common;

namespace University.Domain.Aggregates.Theses;

public sealed class ThesisProject : Entity
{
    private readonly List<ThesisUpdate> _updates = new();

    public ThesisProject(
        Guid studentId,
        Guid supervisorId,
        string title,
        string summary)
    {
        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student id must be provided", nameof(studentId));
        }

        if (supervisorId == Guid.Empty)
        {
            throw new ArgumentException("Supervisor id must be provided", nameof(supervisorId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title must be provided", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException("Summary must be provided", nameof(summary));
        }

        StudentId = studentId;
        SupervisorId = supervisorId;
        Title = title;
        Summary = summary;
        Status = ThesisStatuses.Draft;
    }

    private ThesisProject()
    {
        Title = string.Empty;
        Summary = string.Empty;
        Status = ThesisStatuses.Draft;
    }

    public Guid StudentId { get; private set; }

    public Guid SupervisorId { get; private set; }

    public string Title { get; private set; }

    public string Summary { get; private set; }

    public string Status { get; private set; }

    public IReadOnlyCollection<ThesisUpdate> Updates => _updates.AsReadOnly();

    public void ReassignSupervisor(Guid supervisorId)
    {
        if (supervisorId == Guid.Empty)
        {
            throw new ArgumentException("Supervisor id must be provided", nameof(supervisorId));
        }

        SupervisorId = supervisorId;
    }

    public void UpdateSummary(string summary)
    {
        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException("Summary must be provided", nameof(summary));
        }

        Summary = summary;
    }

    public void ChangeStatus(string status)
    {
        if (!ThesisStatuses.All.Contains(status))
        {
            throw new ArgumentException("Unknown thesis status", nameof(status));
        }

        Status = status;
    }

    public ThesisUpdate AddUpdate(
        Guid authorId,
        string note,
        DateTime occurredOn,
        string status,
        IReadOnlyCollection<ThesisAttachment>? attachments = null)
    {
        EnsureStudentAuthor(authorId);

        var update = new ThesisUpdate(authorId, note, occurredOn, status, attachments);
        _updates.Add(update);
        return update;
    }

    public ThesisUpdate SaveOrUpdateProgress(
        Guid authorId,
        string note,
        DateTime occurredOn,
        string status,
        Guid? updateId = null,
        IReadOnlyCollection<ThesisAttachment>? attachments = null)
    {
        if (updateId is null || updateId == Guid.Empty)
        {
            return AddUpdate(authorId, note, occurredOn, status, attachments);
        }

        var update = FindUpdate(updateId.Value)
            ?? throw new InvalidOperationException("The requested update does not exist for this thesis project.");

        if (update.AuthorId != authorId)
        {
            throw new InvalidOperationException("Only the original author can modify this thesis update.");
        }

        update.UpdateNote(note);
        update.Reschedule(occurredOn);
        update.ChangeStatus(status);
        update.ReplaceAttachments(attachments);
        return update;
    }

    public bool IsParticipant(Guid userId) => userId == StudentId || userId == SupervisorId;

    public bool IsSupervisor(Guid userId) => userId == SupervisorId;

    public ThesisUpdateComment AddFeedback(
        Guid actorId,
        Guid updateId,
        string content,
        Guid? parentCommentId = null)
    {
        EnsureParticipant(actorId);

        var update = FindUpdate(updateId)
            ?? throw new InvalidOperationException("The requested update does not exist for this thesis project.");

        return update.AddComment(actorId, content, parentCommentId);
    }

    public ThesisUpdateComment EditFeedback(Guid actorId, Guid updateId, Guid commentId, string content)
    {
        EnsureParticipant(actorId);

        var update = FindUpdate(updateId)
            ?? throw new InvalidOperationException("The requested update does not exist for this thesis project.");

        return update.EditComment(commentId, actorId, content);
    }

    public ThesisUpdateAuditEntry ChangeUpdateStatus(
        Guid actorId,
        Guid updateId,
        string status,
        string? details = null)
    {
        EnsureSupervisorActor(actorId);

        var update = FindUpdate(updateId)
            ?? throw new InvalidOperationException("The requested update does not exist for this thesis project.");

        var previousStatus = update.Status;

        update.ChangeStatus(status);

        return update.AddAuditEntry(actorId, "StatusChanged", details, previousStatus, update.Status);
    }

    public ThesisUpdateAuditEntry LogUpdateAction(
        Guid actorId,
        Guid updateId,
        string action,
        string? details = null,
        string? fromStatus = null,
        string? toStatus = null)
    {
        EnsureParticipant(actorId);

        var update = FindUpdate(updateId)
            ?? throw new InvalidOperationException("The requested update does not exist for this thesis project.");

        return update.AddAuditEntry(actorId, action, details, fromStatus, toStatus);
    }

    private void EnsureStudentAuthor(Guid authorId)
    {
        if (authorId != StudentId)
        {
            throw new InvalidOperationException("Only the thesis student can author progress updates.");
        }
    }

    private void EnsureParticipant(Guid userId)
    {
        if (!IsParticipant(userId))
        {
            throw new UnauthorizedAccessException("Only thesis participants can perform this operation.");
        }
    }

    private void EnsureSupervisorActor(Guid userId)
    {
        if (!IsSupervisor(userId))
        {
            throw new UnauthorizedAccessException("Only the supervising professor can perform this action.");
        }
    }

    private ThesisUpdate? FindUpdate(Guid updateId) => _updates.FirstOrDefault(existing => existing.Id == updateId);
}
