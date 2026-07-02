using Microsoft.EntityFrameworkCore;
using University.Domain.Enums;
using University.Infrastructure.Data;

namespace University.Web.Services;

public interface IStudentDashboardService
{
    Task<StudentDashboardData> GetDashboardAsync(string? userName, CancellationToken cancellationToken = default);
}

public sealed record StudentDashboardData(
    int? StudentId,
    IReadOnlyList<StudentThesisCard> ActiveTheses,
    IReadOnlyList<DashboardCommentItem> RecentComments,
    IReadOnlyList<DashboardActionItem> ActionItems,
    IReadOnlyList<DashboardFileRecord> FileLibraryRecords)
{
    public static StudentDashboardData Empty { get; } = new(null, [], [], [], []);
}

public sealed record StudentThesisCard(
    Guid TopicId,
    string TopicTitle,
    string Status,
    DateTime NextMilestoneDueOn,
    string? SupervisorName);

public sealed record DashboardCommentItem(
    int UpdateId,
    string Author,
    string Message,
    DateTime CreatedOn);

public sealed record DashboardActionItem(
    string Title,
    string Description,
    DateTime? DueOn,
    string Href);

public sealed record DashboardFileRecord(
    Guid ArtifactId,
    Guid ThesisId,
    string FileName,
    long FileSizeBytes,
    DateTime UploadedOn,
    string DownloadHref);

public sealed class StudentDashboardService : IStudentDashboardService
{
    private const int MilestoneCycleDays = 14;
    private readonly UniversityDbContext _db;

    private sealed record ThesisUpdateSnapshot(
        int Id,
        DateTime SubmittedAt,
        UpdateStatus Status);

    public StudentDashboardService(UniversityDbContext db)
    {
        _db = db;
    }

    public async Task<StudentDashboardData> GetDashboardAsync(string? userName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return StudentDashboardData.Empty;
        }

        var normalizedUser = userName.Trim().ToUpperInvariant();
        var identityUser = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.NormalizedEmail == normalizedUser || u.NormalizedUserName == normalizedUser,
                cancellationToken);

        if (identityUser is null)
        {
            return StudentDashboardData.Empty;
        }

        var student = await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == identityUser.Id)
            .Select(s => new { s.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null)
        {
            return StudentDashboardData.Empty;
        }

        var assignments = await _db.ThesisTopicAssignments
            .AsNoTracking()
            .Where(a => a.StudentId == student.Id)
            .Join(
                _db.Professors.AsNoTracking(),
                assignment => assignment.ProfessorId,
                professor => professor.Id,
                (assignment, professor) => new
                {
                    assignment.TopicId,
                    assignment.TopicTitle,
                    assignment.Status,
                    assignment.AssignedOn,
                    SupervisorUserId = professor.UserId,
                })
            .GroupJoin(
                _db.Users.AsNoTracking(),
                left => left.SupervisorUserId,
                user => user.Id,
                (left, users) => new
                {
                    left.TopicId,
                    left.TopicTitle,
                    left.Status,
                    left.AssignedOn,
                    SupervisorName = users.Select(u => u.Email).FirstOrDefault(),
                })
            .ToListAsync(cancellationToken);

        var updates = await _db.ThesisUpdates
            .AsNoTracking()
            .Where(u => u.StudentId == student.Id)
            .Select(u => new ThesisUpdateSnapshot(
                u.Id,
                u.SubmittedAt,
                u.Status))
            .ToListAsync(cancellationToken);

        var latestUpdateDate = updates.Count == 0 ? (DateTime?)null : updates.Max(u => u.SubmittedAt);
        var thesisCards = assignments
            .Select(a => new StudentThesisCard(
                a.TopicId,
                a.TopicTitle,
                a.Status,
                (latestUpdateDate ?? a.AssignedOn).Date.AddDays(MilestoneCycleDays),
                a.SupervisorName))
            .OrderBy(card => card.NextMilestoneDueOn)
            .ToList();

        var feedback = await _db.Feedback
            .AsNoTracking()
            .Join(
                _db.ThesisUpdates.AsNoTracking().Where(u => u.StudentId == student.Id),
                f => f.UpdateId,
                u => u.Id,
                (f, u) => new { f.UpdateId, f.Comment, f.SubmittedAt, f.ProfessorId })
            .Join(
                _db.Professors.AsNoTracking(),
                left => left.ProfessorId,
                professor => professor.Id,
                (left, professor) => new
                {
                    left.UpdateId,
                    left.Comment,
                    left.SubmittedAt,
                    SupervisorUserId = professor.UserId,
                })
            .GroupJoin(
                _db.Users.AsNoTracking(),
                left => left.SupervisorUserId,
                user => user.Id,
                (left, users) => new DashboardCommentItem(
                    left.UpdateId,
                    users.Select(u => u.Email ?? "Supervisor").FirstOrDefault() ?? "Supervisor",
                    left.Comment,
                    left.SubmittedAt))
            .OrderByDescending(c => c.CreatedOn)
            .Take(5)
            .ToListAsync(cancellationToken);

        var actionItems = BuildActionItems(student.Id, thesisCards, updates);
        var fileRecords = await _db.ThesisArtifacts
            .AsNoTracking()
            .Where(a => a.StudentId == student.Id)
            .OrderByDescending(a => a.UploadedOn)
            .Select(a => new DashboardFileRecord(
                a.ArtifactId,
                a.ThesisId,
                a.FileName,
                a.FileSizeBytes,
                a.UploadedOn,
                $"/api/thesis-artifacts/{a.ArtifactId}"))
            .ToListAsync(cancellationToken);

        return new StudentDashboardData(student.Id, thesisCards, feedback, actionItems, fileRecords);
    }

    private static IReadOnlyList<DashboardActionItem> BuildActionItems(
        int studentId,
        IReadOnlyList<StudentThesisCard> thesisCards,
        IReadOnlyList<ThesisUpdateSnapshot> updates)
    {
        var items = new List<DashboardActionItem>();

        if (updates.Count == 0)
        {
            items.Add(new DashboardActionItem(
                "Publish your first progress update",
                "No thesis updates are submitted yet. Share your first status update for supervisor review.",
                DateTime.UtcNow.Date.AddDays(7),
                $"/updates?authorId={studentId}"));
        }

        foreach (var submitted in updates.Where(u => u.Status == UpdateStatus.Submitted).Take(3))
        {
            items.Add(new DashboardActionItem(
                "Await supervisor review",
                $"Submitted update #{submitted.Id} is pending feedback.",
                submitted.SubmittedAt.Date.AddDays(7),
                $"/updates?authorId={studentId}"));
        }

        foreach (var thesis in thesisCards.Where(card => card.NextMilestoneDueOn <= DateTime.UtcNow.Date.AddDays(10)).Take(2))
        {
            items.Add(new DashboardActionItem(
                $"Upcoming milestone: {thesis.TopicTitle}",
                "Prepare the next milestone deliverable and update your timeline.",
                thesis.NextMilestoneDueOn,
                $"/updates?thesisId={thesis.TopicId}&authorId={studentId}"));
        }

        if (items.Count == 0)
        {
            items.Add(new DashboardActionItem(
                "Keep momentum",
                "Everything is currently on track. Post a new update to keep your thesis timeline fresh.",
                null,
                $"/updates?authorId={studentId}"));
        }

        return items;
    }
}