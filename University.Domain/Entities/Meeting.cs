using System;

namespace University.Domain.Entities;

public class Meeting
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ThesisProjectId { get; set; }
    public DateTime ScheduledForUtc { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(30);
    public string Location { get; set; } = string.Empty;
    public string Agenda { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public MeetingStatus Status { get; set; } = MeetingStatus.Pending;
}

public enum MeetingStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3
}