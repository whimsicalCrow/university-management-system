namespace University.IntegrationTests;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using University.Application.Commands;
using University.Domain.Entities;
using University.Domain.Enums;
using University.Infrastructure.Data;
using University.Infrastructure.Repositories;
using University.Web.Services;

/// <summary>
/// Integration tests for the professor feedback loop (US-022).
/// Uses the EF Core InMemory provider — no real database required.
/// </summary>
public class FeedbackPersistenceTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static UniversityDbContext CreateDbContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new UniversityDbContext(options);
    }

    private static async Task<(IdentityUser StudentUser, IdentityUser ProfUser, Student Student, Professor Professor, ThesisTopicAssignment Assignment, ThesisUpdate Update)>
        SeedAsync(UniversityDbContext db)
    {
        var studentUser = new IdentityUser
        {
            Id = "student-uid-1",
            UserName = "student@univ.edu",
            NormalizedUserName = "STUDENT@UNIV.EDU",
            Email = "student@univ.edu",
            NormalizedEmail = "STUDENT@UNIV.EDU",
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };

        var profUser = new IdentityUser
        {
            Id = "prof-uid-1",
            UserName = "prof@univ.edu",
            NormalizedUserName = "PROF@UNIV.EDU",
            Email = "prof@univ.edu",
            NormalizedEmail = "PROF@UNIV.EDU",
            SecurityStamp = Guid.NewGuid().ToString("N"),
        };

        db.Users.AddRange(studentUser, profUser);

        var student = Student.Create(studentUser.Id, "Computer Science");
        var professor = Professor.Create(profUser.Id, "CS Dept", "Distributed Systems");
        db.Students.Add(student);
        db.Professors.Add(professor);
        await db.SaveChangesAsync();

        var thesis = ThesisTopicAssignment.Create(Guid.NewGuid(), "Test Thesis", student.Id, professor.Id);
        db.ThesisTopicAssignments.Add(thesis);

        var update = ThesisUpdate.Create(student.Id, "Progress update content", "Progress Title", UpdateStatus.Submitted);
        db.ThesisUpdates.Add(update);
        await db.SaveChangesAsync();

        return (studentUser, profUser, student, professor, thesis, update);
    }

    // ── AC: professor approves → status transitions to Reviewed ─────────────

    [Fact]
    public async Task SubmitReview_Approve_TransitionsUpdateToReviewed()
    {
        var dbName = nameof(SubmitReview_Approve_TransitionsUpdateToReviewed);
        await using var db = CreateDbContext(dbName);
        var (_, profUser, _, _, _, update) = await SeedAsync(db);

        var repo = new ThesisUpdateRepository(db);
        var handler = new SubmitReviewCommandHandler(repo, NullLogger<SubmitReviewCommandHandler>.Instance);

        var command = new SubmitReviewCommand
        {
            ProfessorUserName = profUser.Email!,
            UpdateId = update.Id,
            Comment = null,
            NewStatus = "Approved",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);

        // Reload from DB to verify persistence
        await using var verifyDb = CreateDbContext(dbName);
        var persisted = await verifyDb.ThesisUpdates.FirstAsync(u => u.Id == update.Id);
        Assert.Equal(UpdateStatus.Reviewed, persisted.Status);
    }

    // ── AC: professor requests revision → status stays Submitted ─────────────

    [Fact]
    public async Task SubmitReview_NeedsRevision_LeavesUpdateSubmitted()
    {
        var dbName = nameof(SubmitReview_NeedsRevision_LeavesUpdateSubmitted);
        await using var db = CreateDbContext(dbName);
        var (_, profUser, _, _, _, update) = await SeedAsync(db);

        var repo = new ThesisUpdateRepository(db);
        var handler = new SubmitReviewCommandHandler(repo, NullLogger<SubmitReviewCommandHandler>.Instance);

        var command = new SubmitReviewCommand
        {
            ProfessorUserName = profUser.Email!,
            UpdateId = update.Id,
            Comment = null,
            NewStatus = "NeedsRevision",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);

        await using var verifyDb = CreateDbContext(dbName);
        var persisted = await verifyDb.ThesisUpdates.FirstAsync(u => u.Id == update.Id);
        Assert.Equal(UpdateStatus.Submitted, persisted.Status);
    }

    // ── AC: professor submits comment → Feedback row persisted ───────────────

    [Fact]
    public async Task SubmitReview_WithComment_CreatesFeedbackRow()
    {
        var dbName = nameof(SubmitReview_WithComment_CreatesFeedbackRow);
        await using var db = CreateDbContext(dbName);
        var (_, profUser, _, professor, _, update) = await SeedAsync(db);

        var repo = new ThesisUpdateRepository(db);
        var handler = new SubmitReviewCommandHandler(repo, NullLogger<SubmitReviewCommandHandler>.Instance);

        var command = new SubmitReviewCommand
        {
            ProfessorUserName = profUser.Email!,
            UpdateId = update.Id,
            Comment = "Good progress!",
            NewStatus = "Approved",
        };

        await handler.Handle(command, CancellationToken.None);

        // Reload from DB to verify Feedback row persisted
        await using var verifyDb = CreateDbContext(dbName);
        var feedback = await verifyDb.Feedback
            .Where(f => f.UpdateId == update.Id && f.ProfessorId == professor.Id)
            .ToListAsync();

        Assert.Single(feedback);
        Assert.Equal("Good progress!", feedback[0].Comment);
        Assert.Equal(professor.Id, feedback[0].ProfessorId);
    }

    // ── AC: LoadTimelineAsync returns update with professor comments ──────────

    [Fact]
    public async Task LoadTimeline_AfterFeedback_ReturnsFeedbackInComments()
    {
        var dbName = nameof(LoadTimeline_AfterFeedback_ReturnsFeedbackInComments);
        await using var db = CreateDbContext(dbName);
        var (studentUser, profUser, student, professor, _, update) = await SeedAsync(db);

        // Persist feedback directly (simulating a previous review)
        var feedback = Feedback.Create(update.Id, professor.Id, "Revise chapter 3.");
        db.Feedback.Add(feedback);
        await db.SaveChangesAsync();

        var service = new ThesisTimelineService(db);
        var timeline = await service.LoadTimelineAsync(student.Id);

        Assert.Single(timeline);
        var entry = timeline[0];
        Assert.Single(entry.Comments);
        Assert.Equal("Revise chapter 3.", entry.Comments[0].Message);
    }

    // ── AC: ResolveStudentIdAsync returns correct student pk ─────────────────

    [Fact]
    public async Task ResolveStudentIdAsync_KnownUser_ReturnsDomainStudentId()
    {
        var dbName = nameof(ResolveStudentIdAsync_KnownUser_ReturnsDomainStudentId);
        await using var db = CreateDbContext(dbName);
        var (studentUser, _, student, _, _, _) = await SeedAsync(db);

        var service = new ThesisTimelineService(db);
        var resolvedId = await service.ResolveStudentIdAsync(studentUser.Email!);

        Assert.Equal(student.Id, resolvedId);
    }

    // ── AC: UpsertUpdateAsync creates a new Draft record ─────────────────────

    [Fact]
    public async Task UpsertUpdateAsync_NewRecord_PersistsAndReturnsId()
    {
        var dbName = nameof(UpsertUpdateAsync_NewRecord_PersistsAndReturnsId);
        await using var db = CreateDbContext(dbName);
        var (_, _, student, _, _, _) = await SeedAsync(db);

        var service = new ThesisTimelineService(db);
        var savedId = await service.UpsertUpdateAsync(
            student.Id,
            existingId: null,
            title: "New Draft",
            content: "Draft content here",
            status: UpdateStatus.Draft);

        Assert.True(savedId > 0);

        await using var verifyDb = CreateDbContext(dbName);
        var row = await verifyDb.ThesisUpdates.FirstAsync(u => u.Id == savedId);
        Assert.Equal("New Draft", row.Title);
        Assert.Equal(UpdateStatus.Draft, row.Status);
    }
}
