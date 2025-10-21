using System;

namespace University.Domain.Entities;

public class ThesisUpdate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ThesisProjectId { get; set; }
    public string AuthorRole { get; set; } = string.Empty; // e.g., "Student" or "Professor"
    public string Notes { get; set; } = string.Empty;
    public string? ArtifactUri { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAtUtc { get; set; }
}