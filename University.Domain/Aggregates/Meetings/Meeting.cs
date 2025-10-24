using System;
using System.Collections.Generic;
using System.Linq;
using University.Domain.Common;

namespace University.Domain.Aggregates.Meetings;

public sealed class Meeting : Entity
{
    private readonly List<MeetingSlot> _slots = new();
    private readonly List<MeetingActionItem> _actionItems = new();

    public Meeting(
        Guid thesisProjectId,
        Guid studentId,
        Guid supervisorId,
        string agenda,
        IEnumerable<MeetingSlot> proposedSlots,
        string? videoConferenceUrl = null,
        IEnumerable<MeetingActionItem>? actionItems = null)
    {
        if (thesisProjectId == Guid.Empty)
        {
            throw new ArgumentException("Thesis project id must be provided.", nameof(thesisProjectId));
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student id must be provided.", nameof(studentId));
        }

        if (supervisorId == Guid.Empty)
        {
            throw new ArgumentException("Supervisor id must be provided.", nameof(supervisorId));
        }

        if (string.IsNullOrWhiteSpace(agenda))
        {
            throw new ArgumentException("Agenda must be provided.", nameof(agenda));
        }

        ThesisProjectId = thesisProjectId;
        StudentId = studentId;
        SupervisorId = supervisorId;
        Agenda = agenda;
        VideoConferenceUrl = videoConferenceUrl;
        Status = MeetingStatuses.Proposed;
        CreatedOn = DateTime.UtcNow;
        LastUpdatedOn = CreatedOn;

        var slots = proposedSlots?.ToList() ?? throw new ArgumentException("At least one meeting slot must be provided.", nameof(proposedSlots));

        if (slots.Count == 0)
        {
            throw new ArgumentException("At least one meeting slot must be provided.", nameof(proposedSlots));
        }

        foreach (var slot in slots)
        {
            AddSlotInternal(slot);
        }

        if (actionItems is not null)
        {
            foreach (var actionItem in actionItems)
            {
                AddActionItemInternal(actionItem);
            }
        }
    }

    private Meeting()
    {
        Agenda = string.Empty;
        Status = MeetingStatuses.Proposed;
    }

    public Guid ThesisProjectId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid SupervisorId { get; private set; }

    public string Agenda { get; private set; }

    public string Status { get; private set; }

    public Guid? ConfirmedSlotId { get; private set; }

    public string? VideoConferenceUrl { get; private set; }

    public DateTime CreatedOn { get; private set; }

    public DateTime LastUpdatedOn { get; private set; }

    public DateTime? ConfirmedOn { get; private set; }

    public IReadOnlyCollection<MeetingSlot> Slots => _slots.AsReadOnly();

    public IReadOnlyCollection<MeetingActionItem> ActionItems => _actionItems.AsReadOnly();

    public bool IsConfirmed => Status == MeetingStatuses.Confirmed;

    public MeetingSlot ProposeSlot(Guid actorId, DateTime startOn, DateTime endOn, string? note = null)
    {
        EnsureParticipant(actorId);

        if (Status == MeetingStatuses.Cancelled)
        {
            throw new InvalidOperationException("Cannot propose slots for a cancelled meeting.");
        }

        Status = MeetingStatuses.Proposed;

        var slot = new MeetingSlot(actorId, startOn, endOn, MeetingSlotStatuses.Proposed, note);
        AddSlotInternal(slot);
        UpdateLastUpdated();
        return slot;
    }

    public MeetingActionItem AddActionItem(Guid actorId, Guid ownerId, string description, DateTime? dueOnUtc = null)
    {
        EnsureParticipant(actorId);
        EnsureParticipant(ownerId);

        var actionItem = new MeetingActionItem(actorId, ownerId, description, dueOnUtc);
        AddActionItemInternal(actionItem);
        UpdateLastUpdated();
        return actionItem;
    }

    public MeetingActionItem UpdateActionItem(Guid actorId, Guid actionItemId, Guid ownerId, string description, DateTime? dueOnUtc)
    {
        EnsureParticipant(actorId);
        EnsureParticipant(ownerId);

        var actionItem = FindActionItem(actionItemId) ?? throw new InvalidOperationException("The requested action item does not exist.");

        actionItem.Update(ownerId, description, dueOnUtc);
        UpdateLastUpdated();
        return actionItem;
    }

    public MeetingActionItem CompleteActionItem(Guid actorId, Guid actionItemId)
    {
        EnsureParticipant(actorId);

        var actionItem = FindActionItem(actionItemId) ?? throw new InvalidOperationException("The requested action item does not exist.");

        actionItem.Complete();
        UpdateLastUpdated();
        return actionItem;
    }

    public MeetingActionItem ReopenActionItem(Guid actorId, Guid actionItemId)
    {
        EnsureParticipant(actorId);

        var actionItem = FindActionItem(actionItemId) ?? throw new InvalidOperationException("The requested action item does not exist.");

        actionItem.Reopen();
        UpdateLastUpdated();
        return actionItem;
    }

    public void AcceptSlot(Guid actorId, Guid slotId, string? videoConferenceUrl = null)
    {
        EnsureSupervisor(actorId);

        if (IsConfirmed)
        {
            throw new InvalidOperationException("Meeting has already been confirmed.");
        }

        var slot = FindSlot(slotId) ?? throw new InvalidOperationException("The requested meeting slot does not exist.");

        slot.Accept();
        ConfirmedSlotId = slot.Id;
        Status = MeetingStatuses.Confirmed;
        ConfirmedOn = DateTime.UtcNow;
        VideoConferenceUrl = videoConferenceUrl ?? VideoConferenceUrl;

        foreach (var otherSlot in _slots.Where(existing => existing.Id != slot.Id))
        {
            otherSlot.Decline();
        }

        UpdateLastUpdated();
    }

    public void DeclineSlot(Guid actorId, Guid slotId, string? responseNote = null)
    {
        EnsureSupervisor(actorId);

        var slot = FindSlot(slotId) ?? throw new InvalidOperationException("The requested meeting slot does not exist.");

        slot.Decline(responseNote);
        UpdateLastUpdated();

        if (_slots.All(existing => existing.Status == MeetingSlotStatuses.Declined))
        {
            Status = MeetingStatuses.Declined;
        }
    }

    public MeetingSlot SuggestAlternativeSlot(Guid actorId, DateTime startOn, DateTime endOn, string? note = null)
    {
        EnsureSupervisor(actorId);

        return ProposeSlot(actorId, startOn, endOn, note);
    }

    public void Cancel(Guid actorId)
    {
        EnsureParticipant(actorId);

        if (Status == MeetingStatuses.Cancelled)
        {
            return;
        }

        Status = MeetingStatuses.Cancelled;
        UpdateLastUpdated();
    }

    public void UpdateAgenda(string agenda)
    {
        if (string.IsNullOrWhiteSpace(agenda))
        {
            throw new ArgumentException("Agenda must be provided.", nameof(agenda));
        }

        Agenda = agenda;
        UpdateLastUpdated();
    }

    private void AddSlotInternal(MeetingSlot slot)
    {
        if (slot is null)
        {
            throw new ArgumentNullException(nameof(slot));
        }

        _slots.Add(slot);
    }

    private MeetingSlot? FindSlot(Guid slotId)
    {
        return _slots.FirstOrDefault(slot => slot.Id == slotId);
    }

    private MeetingActionItem? FindActionItem(Guid actionItemId)
    {
        return _actionItems.FirstOrDefault(item => item.Id == actionItemId);
    }

    private void EnsureParticipant(Guid userId)
    {
        if (userId != StudentId && userId != SupervisorId)
        {
            throw new UnauthorizedAccessException("Only meeting participants may perform this action.");
        }
    }

    private void EnsureSupervisor(Guid userId)
    {
        if (userId != SupervisorId)
        {
            throw new UnauthorizedAccessException("Only the supervising professor may perform this action.");
        }
    }

    private void UpdateLastUpdated()
    {
        LastUpdatedOn = DateTime.UtcNow;
    }

    private void AddActionItemInternal(MeetingActionItem actionItem)
    {
        if (actionItem is null)
        {
            throw new ArgumentNullException(nameof(actionItem));
        }

        EnsureParticipant(actionItem.CreatedById);
        EnsureParticipant(actionItem.OwnerId);

        _actionItems.Add(actionItem);
    }
}