using System;
using University.Domain.Aggregates.Theses;

namespace University.UnitTests;

public class ThesisProjectTests
{
    [Fact]
    public void SaveOrUpdateProgress_Throws_WhenAuthorIsNotStudent()
    {
        var thesis = new ThesisProject(
            studentId: Guid.NewGuid(),
            supervisorId: Guid.NewGuid(),
            title: "AI for Smart Grids",
            summary: "Initial thesis summary");

        var invalidAuthorId = Guid.NewGuid();

        var act = () => thesis.SaveOrUpdateProgress(
            authorId: invalidAuthorId,
            note: "Weekly update",
            occurredOn: DateTime.UtcNow,
            status: ThesisUpdateStatuses.PendingReview);

        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void SaveOrUpdateProgress_UpdatesExistingEntry()
    {
        var studentId = Guid.NewGuid();
        var thesis = new ThesisProject(
            studentId,
            Guid.NewGuid(),
            "AI for Smart Grids",
            "Initial thesis summary");

        var existing = thesis.SaveOrUpdateProgress(
            authorId: studentId,
            note: "Week 1 progress",
            occurredOn: DateTime.UtcNow.AddDays(-1),
            status: ThesisUpdateStatuses.Draft);

        var updated = thesis.SaveOrUpdateProgress(
            authorId: studentId,
            note: "Week 1 progress - refined",
            occurredOn: DateTime.UtcNow,
            status: ThesisUpdateStatuses.PendingReview,
            updateId: existing.Id);

        Assert.Equal(existing.Id, updated.Id);
        Assert.Equal("Week 1 progress - refined", updated.Note);
        Assert.Equal(ThesisUpdateStatuses.PendingReview, updated.Status);
        Assert.True(updated.LastModifiedOn >= updated.OccurredOn);
    }
}
