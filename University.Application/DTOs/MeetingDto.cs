using System;

namespace University.Application.DTOs;

public record MeetingDto(
    Guid Id,
    Guid ThesisProjectId,
    DateTime ScheduledForUtc,
    TimeSpan Duration,
    string Location,
    string Agenda,
    string? Notes,
    string Status
);