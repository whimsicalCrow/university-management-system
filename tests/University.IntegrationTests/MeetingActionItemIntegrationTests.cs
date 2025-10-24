using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using University.Application.DependencyInjection;
using University.Application.Meetings.Commands.AddMeetingActionItem;
using University.Application.Meetings.Commands.SetMeetingActionItemCompletion;
using University.Application.Meetings.Commands.UpdateMeetingActionItem;
using University.Application.Meetings.Notifications;
using University.Application.Meetings.Shared;
using University.Domain.Aggregates.Meetings;
using University.Domain.Repositories;
using University.Infrastructure.Persistence;
using University.Infrastructure.Repositories;

namespace University.IntegrationTests;

public sealed class MeetingActionItemIntegrationTests
{
    [Fact]
    public async Task Participant_actions_emit_broadcast_events()
    {
        await using var provider = BuildServiceProvider(out var broadcaster);
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var repository = scope.ServiceProvider.GetRequiredService<IMeetingRepository>();

        var (meetingId, studentId, supervisorId) = await SeedMeetingAsync(repository);

        var addResult = await mediator.Send(new AddMeetingActionItemCommand(
            meetingId,
            studentId,
            studentId,
            "Prepare draft outline",
            DateTime.UtcNow.AddDays(2)));

    var actionItem = addResult.ActionItems.Single();

        var updatedDescription = "Prepare draft outline with bibliography";

        await mediator.Send(new UpdateMeetingActionItemCommand(
            meetingId,
            actionItem.Id,
            supervisorId,
            supervisorId,
            updatedDescription,
            actionItem.DueOnUtc));

        await mediator.Send(new SetMeetingActionItemCompletionCommand(
            meetingId,
            actionItem.Id,
            supervisorId,
            true));

        Assert.Collection(
            broadcaster.Events,
            added =>
            {
                Assert.Equal("ActionItemAdded", added.EventName);
                Assert.Equal(meetingId, added.MeetingId);
                Assert.Equal(actionItem.Id, added.ActionItem.Id);
            },
            updated =>
            {
                Assert.Equal("ActionItemUpdated", updated.EventName);
                Assert.Equal(meetingId, updated.MeetingId);
                Assert.Equal(updatedDescription, updated.ActionItem.Description);
            },
            statusChanged =>
            {
                Assert.Equal("ActionItemStatusChanged", statusChanged.EventName);
                Assert.Equal(meetingId, statusChanged.MeetingId);
                Assert.Equal(MeetingActionItemStatuses.Completed, statusChanged.ActionItem.Status);
            });
    }

    [Fact]
    public async Task Unauthorized_participant_cannot_modify_action_items()
    {
        await using var provider = BuildServiceProvider(out var broadcaster);
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var repository = scope.ServiceProvider.GetRequiredService<IMeetingRepository>();

        var (meetingId, studentId, supervisorId) = await SeedMeetingAsync(repository);

        var addResult = await mediator.Send(new AddMeetingActionItemCommand(
            meetingId,
            studentId,
            studentId,
            "Prepare slides",
            null));

        var actionItem = addResult.ActionItems.Single();
        var outsiderId = Guid.NewGuid();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => mediator.Send(new AddMeetingActionItemCommand(
            meetingId,
            outsiderId,
            outsiderId,
            "Intruder item",
            null)));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => mediator.Send(new UpdateMeetingActionItemCommand(
            meetingId,
            actionItem.Id,
            outsiderId,
            studentId,
            "Tampering",
            null)));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => mediator.Send(new SetMeetingActionItemCompletionCommand(
            meetingId,
            actionItem.Id,
            outsiderId,
            true)));

    var addedEvent = Assert.Single(broadcaster.Events);
    Assert.Equal("ActionItemAdded", addedEvent.EventName);
    Assert.Equal(meetingId, addedEvent.MeetingId);
    }

    private static async Task<(Guid MeetingId, Guid StudentId, Guid SupervisorId)> SeedMeetingAsync(IMeetingRepository repository)
    {
        var thesisId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var supervisorId = Guid.NewGuid();
        var start = DateTime.UtcNow.AddDays(1);
        var slot = new MeetingSlot(studentId, start, start.AddHours(1));

        var meeting = new Meeting(thesisId, studentId, supervisorId, "Thesis kickoff", new[] { slot });

        await repository.AddAsync(meeting);

        return (meeting.Id, studentId, supervisorId);
    }

    private static ServiceProvider BuildServiceProvider(out TestMeetingActionItemBroadcaster broadcaster)
    {
        var services = new ServiceCollection();

        services.AddDbContext<UniversityDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<IMeetingRepository, MeetingRepository>();
        services.AddApplicationLayer();

        broadcaster = new TestMeetingActionItemBroadcaster();
        services.AddSingleton<IMeetingActionItemBroadcaster>(broadcaster);

        return services.BuildServiceProvider();
    }

    private sealed class TestMeetingActionItemBroadcaster : IMeetingActionItemBroadcaster
    {
        private readonly List<BroadcastEvent> _events = new();

        public IReadOnlyList<BroadcastEvent> Events => _events;

        public Task ItemAddedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken)
        {
            return RecordAsync("ActionItemAdded", meeting, actionItem);
        }

        public Task ItemUpdatedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken)
        {
            return RecordAsync("ActionItemUpdated", meeting, actionItem);
        }

        public Task ItemStatusChangedAsync(MeetingDto meeting, MeetingActionItemDto actionItem, CancellationToken cancellationToken)
        {
            return RecordAsync("ActionItemStatusChanged", meeting, actionItem);
        }

        private Task RecordAsync(string eventName, MeetingDto meeting, MeetingActionItemDto actionItem)
        {
            _events.Add(new BroadcastEvent(eventName, meeting.Id, actionItem));
            return Task.CompletedTask;
        }

        public readonly record struct BroadcastEvent(string EventName, Guid MeetingId, MeetingActionItemDto ActionItem);
    }
}