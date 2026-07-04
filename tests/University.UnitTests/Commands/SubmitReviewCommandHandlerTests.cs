namespace University.UnitTests.Commands;

using Microsoft.Extensions.Logging.Abstractions;
using University.Application.Commands;
using University.Application.Interfaces;
using University.Domain.Entities;
using University.Domain.Enums;

/// <summary>
/// Unit tests for <see cref="SubmitReviewCommandHandler"/>.
/// All dependencies are replaced by lightweight in-memory stubs; no database required.
/// </summary>
public class SubmitReviewCommandHandlerTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Professor BuildProfessor(int id = 1, string userId = "prof-user-id-1")
    {
        var professor = Professor.Create(userId, "CS Dept", "Distributed Systems");
        typeof(Professor).GetProperty("Id")!.SetValue(professor, id);
        return professor;
    }

    private static ThesisUpdate BuildUpdate(
        int id = 10,
        int studentId = 5,
        UpdateStatus status = UpdateStatus.Submitted)
    {
        var update = ThesisUpdate.Create(studentId, "Test content", "Test title", status);
        typeof(ThesisUpdate).GetProperty("Id")!.SetValue(update, id);
        return update;
    }

    private static SubmitReviewCommandHandler BuildHandler(
        FakeThesisUpdateRepository? repo = null)
    {
        return new SubmitReviewCommandHandler(
            repo ?? new FakeThesisUpdateRepository(),
            NullLogger<SubmitReviewCommandHandler>.Instance);
    }

    // ── AC: professor not found ──────────────────────────────────────────────

    [Fact]
    public async Task Handle_ProfessorNotFound_ReturnsFailure()
    {
        var repo = new FakeThesisUpdateRepository { Professor = null };
        var handler = BuildHandler(repo);
        var command = new SubmitReviewCommand
        {
            ProfessorUserName = "nobody@univ.edu",
            UpdateId = 1,
            NewStatus = "Approved",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    // ── AC: update not found ─────────────────────────────────────────────────

    [Fact]
    public async Task Handle_UpdateNotFound_ReturnsFailure()
    {
        var repo = new FakeThesisUpdateRepository
        {
            Professor = BuildProfessor(),
            Update = null,
        };
        var handler = BuildHandler(repo);
        var command = new SubmitReviewCommand
        {
            ProfessorUserName = "prof@univ.edu",
            UpdateId = 999,
            NewStatus = "Approved",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    // ── AC: professor not assigned to student ───────────────────────────────

    [Fact]
    public async Task Handle_ProfessorNotAssigned_ReturnsFailure()
    {
        var repo = new FakeThesisUpdateRepository
        {
            Professor = BuildProfessor(),
            Update = BuildUpdate(),
            IsAssigned = false,
        };
        var handler = BuildHandler(repo);
        var command = new SubmitReviewCommand
        {
            ProfessorUserName = "prof@univ.edu",
            UpdateId = 10,
            NewStatus = "Approved",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    // ── AC: approve without comment ──────────────────────────────────────────

    [Fact]
    public async Task Handle_ApproveWithoutComment_TransitionsStatusAndSucceeds()
    {
        var update = BuildUpdate(status: UpdateStatus.Submitted);
        var repo = new FakeThesisUpdateRepository
        {
            Professor = BuildProfessor(),
            Update = update,
            IsAssigned = true,
        };
        var handler = BuildHandler(repo);
        var command = new SubmitReviewCommand
        {
            ProfessorUserName = "prof@univ.edu",
            UpdateId = update.Id,
            Comment = null,
            NewStatus = "Approved",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(UpdateStatus.Reviewed, update.Status);
        Assert.Empty(repo.AddedFeedback);
        Assert.True(repo.SaveChangesCalled);
    }

    // ── AC: needs-revision with comment ─────────────────────────────────────

    [Fact]
    public async Task Handle_NeedsRevisionWithComment_PersistesFeedbackAndTransitionsStatus()
    {
        var update = BuildUpdate(status: UpdateStatus.Submitted);
        var professor = BuildProfessor(id: 1);
        var repo = new FakeThesisUpdateRepository
        {
            Professor = professor,
            Update = update,
            IsAssigned = true,
        };
        var handler = BuildHandler(repo);
        var command = new SubmitReviewCommand
        {
            ProfessorUserName = "prof@univ.edu",
            UpdateId = update.Id,
            Comment = "Please add more detail.",
            NewStatus = "NeedsRevision",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        // Status reverts to Submitted (RequestRevision)
        Assert.Equal(UpdateStatus.Submitted, update.Status);
        // A Feedback row was persisted
        Assert.Single(repo.AddedFeedback);
        Assert.Equal("Please add more detail.", repo.AddedFeedback[0].Comment);
        Assert.True(repo.SaveChangesCalled);
    }

    // ── AC: approve with comment ─────────────────────────────────────────────

    [Fact]
    public async Task Handle_ApproveWithComment_PersistesFeedbackAndTransitionsToReviewed()
    {
        var update = BuildUpdate(status: UpdateStatus.Submitted);
        var repo = new FakeThesisUpdateRepository
        {
            Professor = BuildProfessor(id: 2),
            Update = update,
            IsAssigned = true,
        };
        var handler = BuildHandler(repo);
        var command = new SubmitReviewCommand
        {
            ProfessorUserName = "prof@univ.edu",
            UpdateId = update.Id,
            Comment = "Excellent work!",
            NewStatus = "Approved",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(UpdateStatus.Reviewed, update.Status);
        Assert.Single(repo.AddedFeedback);
        Assert.Equal("Excellent work!", repo.AddedFeedback[0].Comment);
    }

    // ── AC: invalid NewStatus ────────────────────────────────────────────────

    [Fact]
    public async Task Handle_InvalidNewStatus_ReturnsFailure()
    {
        var repo = new FakeThesisUpdateRepository
        {
            Professor = BuildProfessor(),
            Update = BuildUpdate(),
            IsAssigned = true,
        };
        var handler = BuildHandler(repo);
        var command = new SubmitReviewCommand
        {
            ProfessorUserName = "prof@univ.edu",
            UpdateId = 10,
            Comment = null,
            NewStatus = "InvalidStatus",
        };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    // ── Fake ─────────────────────────────────────────────────────────────────

    private sealed class FakeThesisUpdateRepository : IThesisUpdateRepository
    {
        public Professor? Professor { get; init; } = Professor.Create("prof-user-id-1", "CS Dept", "Distributed Systems");
        public ThesisUpdate? Update { get; init; }
        public bool IsAssigned { get; init; } = true;
        public List<Feedback> AddedFeedback { get; } = [];
        public bool SaveChangesCalled { get; private set; }

        public Task<ThesisUpdate?> GetByIdAsync(int id, CancellationToken ct = default) =>
            Task.FromResult(Update);

        public Task<Professor?> FindProfessorByEmailAsync(string email, CancellationToken ct = default) =>
            Task.FromResult(Professor);

        public Task<bool> IsProfessorAssignedToStudentAsync(int professorId, int studentId, CancellationToken ct = default) =>
            Task.FromResult(IsAssigned);

        public Task<int> AddFeedbackAsync(Feedback feedback, CancellationToken ct = default)
        {
            AddedFeedback.Add(feedback);
            return Task.FromResult(AddedFeedback.Count);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            SaveChangesCalled = true;
            return Task.CompletedTask;
        }
    }
}
