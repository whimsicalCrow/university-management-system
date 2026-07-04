namespace University.Web.Services;

using Microsoft.EntityFrameworkCore;
using University.Domain.Entities;
using University.Domain.Enums;
using University.Infrastructure.Data;

/// <summary>
/// Read/write service for the thesis-update timeline.
/// Follows the same EF-direct pattern used by <see cref="StudentDashboardService"/>.
/// </summary>
public interface IThesisTimelineService
{
    /// <summary>
    /// Resolves the domain <see cref="Student.Id"/> (int) from an ASP.NET Core Identity
    /// email/username, or returns null when no student profile exists.
    /// </summary>
    Task<int?> ResolveStudentIdAsync(string identityUserName, CancellationToken ct = default);

    /// <summary>
    /// Returns the full timeline for <paramref name="studentId"/>, ordered newest-first,
    /// with each entry's professor comments hydrated.
    /// </summary>
    Task<IReadOnlyList<ThesisTimelineEntry>> LoadTimelineAsync(
        int studentId,
        string? statusFilter = null,
        bool excludeDrafts = false,
        CancellationToken ct = default);

    /// <summary>
    /// Creates or updates a thesis update for <paramref name="studentId"/>.
    /// When <paramref name="existingId"/> is null a new record is inserted; otherwise the
    /// matching record is updated in-place.
    /// Returns the database primary key.
    /// </summary>
    Task<int> UpsertUpdateAsync(
        int studentId,
        int? existingId,
        string title,
        string content,
        UpdateStatus status,
        CancellationToken ct = default);

    /// <summary>
    /// Links a persisted <see cref="ThesisArtifact"/> to a thesis update and stores the
    /// attachment metadata on the <see cref="ThesisUpdate"/> entity.
    /// </summary>
    Task AttachArtifactAsync(
        int updateId,
        Guid artifactId,
        string fileName,
        long sizeBytes,
        CancellationToken ct = default);
}

/// <summary>DTO produced by <see cref="IThesisTimelineService.LoadTimelineAsync"/>.</summary>
public sealed record ThesisTimelineEntry(
    int Id,
    int AuthorId,
    string Title,
    string Status,
    string Content,
    DateTime OccurredOn,
    DateTime LastModifiedOn,
    string? AttachmentFileName,
    long? AttachmentSizeBytes,
    Guid? ArtifactId,
    IReadOnlyList<ThesisFeedbackComment> Comments);

/// <summary>Professor comment attached to a timeline entry.</summary>
public sealed record ThesisFeedbackComment(
    int Id,
    string Author,
    string Message,
    DateTime CreatedOn);

// ── Implementation ────────────────────────────────────────────────────────────

/// <inheritdoc cref="IThesisTimelineService"/>
public sealed class ThesisTimelineService : IThesisTimelineService
{
    private readonly UniversityDbContext _db;

    public ThesisTimelineService(UniversityDbContext db)
    {
        _db = db;
    }

    // ── Status helpers ────────────────────────────────────────────────────────

    /// <summary>Maps <see cref="UpdateStatus"/> enum to the UI string used in badges/filters.</summary>
    public static string StatusToLabel(UpdateStatus s) => s switch
    {
        UpdateStatus.Draft => "Draft",
        UpdateStatus.Submitted => "Under review",
        UpdateStatus.Reviewed => "Approved",
        _ => s.ToString()
    };

    private static UpdateStatus? LabelToStatus(string? label) => label?.ToLowerInvariant() switch
    {
        "draft" => UpdateStatus.Draft,
        "under review" => UpdateStatus.Submitted,
        "approved" => UpdateStatus.Reviewed,
        _ => null
    };

    // ── IThesisTimelineService ───────────────────────────────────────────────

    public async Task<int?> ResolveStudentIdAsync(
        string identityUserName, CancellationToken ct = default)
    {
        var normalised = identityUserName.Trim().ToUpperInvariant();

        var userId = await _db.Users
            .AsNoTracking()
            .Where(u => u.NormalizedEmail == normalised || u.NormalizedUserName == normalised)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(ct);

        if (userId is null) return null;

        return await _db.Students
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<ThesisTimelineEntry>> LoadTimelineAsync(
        int studentId,
        string? statusFilter = null,
        bool excludeDrafts = false,
        CancellationToken ct = default)
    {
        // Load updates for the student
        var query = _db.ThesisUpdates
            .AsNoTracking()
            .Where(u => u.StudentId == studentId);

        if (excludeDrafts)
            query = query.Where(u => u.Status != UpdateStatus.Draft);

        // Status filter by UI label
        var statusEnum = LabelToStatus(statusFilter);
        if (statusEnum.HasValue)
            query = query.Where(u => u.Status == statusEnum.Value);

        var updates = await query
            .OrderByDescending(u => u.SubmittedAt)
            .ToListAsync(ct);

        if (updates.Count == 0)
            return [];

        var updateIds = updates.Select(u => u.Id).ToList();

        // Load all feedback for these updates in a single query
        var rawFeedback = await _db.Feedback
            .AsNoTracking()
            .Where(f => updateIds.Contains(f.UpdateId))
            .Join(
                _db.Professors.AsNoTracking(),
                f => f.ProfessorId,
                p => p.Id,
                (f, p) => new { f.UpdateId, f.Id, f.Comment, f.SubmittedAt, ProfUserId = p.UserId })
            .GroupJoin(
                _db.Users.AsNoTracking(),
                left => left.ProfUserId,
                u => u.Id,
                (left, users) => new
                {
                    left.UpdateId,
                    left.Id,
                    left.Comment,
                    left.SubmittedAt,
                    Author = users.Select(u => u.Email ?? "Supervisor").FirstOrDefault() ?? "Supervisor"
                })
            .OrderByDescending(f => f.SubmittedAt)
            .ToListAsync(ct);

        var feedbackByUpdate = rawFeedback
            .GroupBy(f => f.UpdateId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<ThesisFeedbackComment>)
                g.Select(f => new ThesisFeedbackComment(f.Id, f.Author, f.Comment, f.SubmittedAt)).ToList());

        return updates.Select(u => new ThesisTimelineEntry(
            u.Id,
            u.StudentId,
            u.Title ?? "(untitled)",
            StatusToLabel(u.Status),
            u.Content,
            u.SubmittedAt,
            u.SubmittedAt,       // LastModifiedOn — same field until we add UpdatedAt
            u.AttachmentFileName,
            u.AttachmentSizeBytes,
            u.ThesisArtifactId,
            feedbackByUpdate.TryGetValue(u.Id, out var comments) ? comments : []
        )).ToList();
    }

    public async Task<int> UpsertUpdateAsync(
        int studentId,
        int? existingId,
        string title,
        string content,
        UpdateStatus status,
        CancellationToken ct = default)
    {
        ThesisUpdate update;

        if (existingId.HasValue)
        {
            var existing = await _db.ThesisUpdates
                .FirstOrDefaultAsync(u => u.Id == existingId.Value && u.StudentId == studentId, ct);

            if (existing is null)
                throw new InvalidOperationException(
                    $"ThesisUpdate #{existingId} not found for student {studentId}.");

            existing.UpdateContent(content, title);

            // Allow Draft ↔ Submitted transitions but never overwrite Reviewed
            existing.SetStatus(status);

            update = existing;
        }
        else
        {
            update = ThesisUpdate.Create(studentId, content, title, status);
            _db.ThesisUpdates.Add(update);
        }

        await _db.SaveChangesAsync(ct);
        return update.Id;
    }

    public async Task AttachArtifactAsync(
        int updateId,
        Guid artifactId,
        string fileName,
        long sizeBytes,
        CancellationToken ct = default)
    {
        var update = await _db.ThesisUpdates.FirstOrDefaultAsync(u => u.Id == updateId, ct);
        if (update is null) return;

        update.LinkArtifact(artifactId);
        update.AttachFileMetadata(fileName, sizeBytes);
        await _db.SaveChangesAsync(ct);
    }
}
