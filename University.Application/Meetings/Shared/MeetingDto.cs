using System;
using System.Collections.Generic;

namespace University.Application.Meetings.Shared;

public sealed record MeetingDto(
    Guid Id,
    Guid ThesisProjectId,
    Guid StudentId,
    Guid SupervisorId,
    string Agenda,
    string Status,
    Guid? ConfirmedSlotId,
    string? VideoConferenceUrl,
    DateTime CreatedOn,
    DateTime LastUpdatedOn,
    DateTime? ConfirmedOn,
    IReadOnlyCollection<MeetingSlotDto> Slots);

public sealed record MeetingSlotDto(
    Guid Id,
    DateTime StartOn,
    DateTime EndOn,
    string Status,
    Guid ProposedById,
    string? Note,
    string? ResponseNote,
    DateTime ProposedOn,
    DateTime? RespondedOn);

public sealed record ProposedMeetingSlotDto(
    DateTime StartOn,
    DateTime EndOn,
    string? Note);