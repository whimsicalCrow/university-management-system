using System;
using System.Collections.Generic;

namespace University.Domain.Entities;

public class ThesisProject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public Guid ProfessorId { get; set; }
    public ThesisProjectStatus Status { get; set; } = ThesisProjectStatus.Draft;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAtUtc { get; set; }
    public ICollection<ThesisUpdate> Updates { get; set; } = new List<ThesisUpdate>();
    public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
}

public enum ThesisProjectStatus
{
    Draft = 0,
    InProgress = 1,
    AwaitingReview = 2,
    Approved = 3,
    Archived = 4
}