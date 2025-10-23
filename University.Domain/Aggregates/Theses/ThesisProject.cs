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

    public ThesisUpdate AddUpdate(Guid authorId, string note, DateTime occurredOn, IReadOnlyCollection<ThesisAttachment>? attachments = null)
    {
        var update = new ThesisUpdate(authorId, note, occurredOn, attachments);
        _updates.Add(update);
        return update;
    }
}
