using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using University.Application.Meetings.Commands.AcceptMeetingSlot;
using University.Application.Meetings.Commands.DeclineMeetingSlot;
using University.Application.Meetings.Commands.ProposeMeeting;
using University.Application.Meetings.Shared;
using University.Domain.Aggregates.Meetings;
using University.Domain.Aggregates.Theses;
using University.Domain.Repositories;

namespace University.UnitTests;

public class MeetingCommandHandlerTests
{
    private readonly InMemoryMeetingRepository _meetingRepository = new();
    private readonly StubThesisProjectRepository _thesisRepository = new();

    [Fact]
    public async Task ProposeMeetingCommandHandler_CreatesMeeting()
    {
        var thesis = CreateThesisProject();
        await _thesisRepository.AddAsync(thesis);

        var command = new ProposeMeetingCommand(
            thesis.Id,
            thesis.StudentId,
            "Discuss implementation milestones",
            new[]
            {
                new ProposedMeetingSlotDto(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(1), "Morning slot"),
                new ProposedMeetingSlotDto(DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(1), null),
            },
            null);

        var handler = new ProposeMeetingCommandHandler(_meetingRepository, _thesisRepository);

        var meeting = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(thesis.Id, meeting.ThesisProjectId);
        Assert.Equal(thesis.SupervisorId, meeting.SupervisorId);
        Assert.Equal(2, meeting.Slots.Count);
        Assert.All(meeting.Slots, slot => Assert.Equal(MeetingSlotStatuses.Proposed, slot.Status));
    }

    [Fact]
    public async Task AcceptMeetingSlotCommandHandler_ConfirmsMeeting()
    {
        var thesis = CreateThesisProject();
        await _thesisRepository.AddAsync(thesis);

        var firstSlotStart = DateTime.UtcNow.AddDays(3);
        var secondSlotStart = DateTime.UtcNow.AddDays(4);

        var meeting = new Meeting(
            thesis.Id,
            thesis.StudentId,
            thesis.SupervisorId,
            "Weekly sync",
            new[]
            {
                new MeetingSlot(thesis.StudentId, firstSlotStart, firstSlotStart.AddHours(1)),
                new MeetingSlot(thesis.StudentId, secondSlotStart, secondSlotStart.AddHours(1)),
            },
            null);

        await _meetingRepository.AddAsync(meeting, CancellationToken.None);

        var acceptHandler = new AcceptMeetingSlotCommandHandler(_meetingRepository);

        var updated = await acceptHandler.Handle(
            new AcceptMeetingSlotCommand(meeting.Id, meeting.Slots.Last().Id, thesis.SupervisorId, "https://teams.example.com/meet"),
            CancellationToken.None);

        Assert.Equal(MeetingStatuses.Confirmed, updated.Status);
        Assert.Equal(meeting.Slots.Last().Id, updated.ConfirmedSlotId);
        Assert.Equal(MeetingSlotStatuses.Accepted, updated.Slots.Single(slot => slot.Id == updated.ConfirmedSlotId).Status);
    }

    [Fact]
    public async Task DeclineMeetingSlotCommandHandler_DeclinesAndSuggestsAlternative()
    {
        var thesis = CreateThesisProject();
        await _thesisRepository.AddAsync(thesis);

        var initialStart = DateTime.UtcNow.AddDays(5);

        var meeting = new Meeting(
            thesis.Id,
            thesis.StudentId,
            thesis.SupervisorId,
            "Architecture review",
            new[]
            {
                new MeetingSlot(thesis.StudentId, initialStart, initialStart.AddHours(1)),
            },
            null);

        await _meetingRepository.AddAsync(meeting, CancellationToken.None);

        var declineHandler = new DeclineMeetingSlotCommandHandler(_meetingRepository);

        var alternativeStart = DateTime.UtcNow.AddDays(6);

        var updated = await declineHandler.Handle(
            new DeclineMeetingSlotCommand(
                meeting.Id,
                meeting.Slots.Single().Id,
                thesis.SupervisorId,
                "Need later in the week",
                new ProposedMeetingSlotDto(alternativeStart, alternativeStart.AddHours(1), "Afternoon preferred")),
            CancellationToken.None);

        Assert.Equal(MeetingSlotStatuses.Declined, updated.Slots.First().Status);
        Assert.Equal(MeetingStatuses.Proposed, updated.Status);
        Assert.Equal(2, updated.Slots.Count);
        Assert.Contains(updated.Slots, slot => slot.Note == "Afternoon preferred");
    }

    [Fact]
    public async Task ProposeMeetingCommandHandler_ThrowsWhenSupervisorIsOverbooked()
    {
        var thesis = CreateThesisProject();
        await _thesisRepository.AddAsync(thesis);

        var existingStart = DateTime.UtcNow.AddDays(2).AddHours(9);

        var existingMeeting = new Meeting(
            thesis.Id,
            thesis.StudentId,
            thesis.SupervisorId,
            "Existing sync",
            new[]
            {
                new MeetingSlot(thesis.StudentId, existingStart, existingStart.AddHours(1)),
            });

        await _meetingRepository.AddAsync(existingMeeting, CancellationToken.None);

        var overlappingCommand = new ProposeMeetingCommand(
            thesis.Id,
            thesis.StudentId,
            "Follow-up discussion",
            new[]
            {
                new ProposedMeetingSlotDto(existingStart.AddMinutes(30), existingStart.AddMinutes(90), null),
            },
            null);

        var handler = new ProposeMeetingCommandHandler(_meetingRepository, _thesisRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(overlappingCommand, CancellationToken.None));

        Assert.Equal("The supervisor is already booked for one of the proposed slots.", exception.Message);
    }

    [Fact]
    public async Task AcceptMeetingSlotCommandHandler_ThrowsWhenSlotOverlapsAnotherMeeting()
    {
        var thesis = CreateThesisProject();
        await _thesisRepository.AddAsync(thesis);

        var baseStart = DateTime.UtcNow.AddDays(3).AddHours(11);

        var blockingMeeting = new Meeting(
            Guid.NewGuid(),
            Guid.NewGuid(),
            thesis.SupervisorId,
            "Design review",
            new[]
            {
                new MeetingSlot(thesis.StudentId, baseStart, baseStart.AddHours(1)),
            });

        await _meetingRepository.AddAsync(blockingMeeting, CancellationToken.None);

        var pendingMeeting = new Meeting(
            thesis.Id,
            thesis.StudentId,
            thesis.SupervisorId,
            "Implementation deep dive",
            new[]
            {
                new MeetingSlot(thesis.StudentId, baseStart.AddMinutes(15), baseStart.AddMinutes(75)),
            });

        await _meetingRepository.AddAsync(pendingMeeting, CancellationToken.None);

        var handler = new AcceptMeetingSlotCommandHandler(_meetingRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new AcceptMeetingSlotCommand(pendingMeeting.Id, pendingMeeting.Slots.Single().Id, thesis.SupervisorId, null),
            CancellationToken.None));

        Assert.Equal("The supervisor is already booked for another meeting during the selected slot.", exception.Message);
    }

    [Fact]
    public async Task DeclineMeetingSlotCommandHandler_ThrowsWhenAlternativeOverlaps()
    {
        var thesis = CreateThesisProject();
        await _thesisRepository.AddAsync(thesis);

        var conflictStart = DateTime.UtcNow.AddDays(4).AddHours(14);

        var conflictingMeeting = new Meeting(
            Guid.NewGuid(),
            Guid.NewGuid(),
            thesis.SupervisorId,
            "Team sync",
            new[]
            {
                new MeetingSlot(thesis.StudentId, conflictStart, conflictStart.AddHours(1)),
            });

        await _meetingRepository.AddAsync(conflictingMeeting, CancellationToken.None);

        var meeting = new Meeting(
            thesis.Id,
            thesis.StudentId,
            thesis.SupervisorId,
            "Sprint planning",
            new[]
            {
                new MeetingSlot(thesis.StudentId, conflictStart.AddDays(1), conflictStart.AddDays(1).AddHours(1)),
            });

        await _meetingRepository.AddAsync(meeting, CancellationToken.None);

        var handler = new DeclineMeetingSlotCommandHandler(_meetingRepository);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new DeclineMeetingSlotCommand(
                meeting.Id,
                meeting.Slots.Single().Id,
                thesis.SupervisorId,
                "Can't make it",
                new ProposedMeetingSlotDto(conflictStart.AddMinutes(30), conflictStart.AddMinutes(90), "Let's try earlier")),
            CancellationToken.None));

        Assert.Equal("The suggested alternative overlaps with another meeting for the supervisor.", exception.Message);

        Assert.Single(meeting.Slots);
        Assert.Equal(MeetingSlotStatuses.Proposed, meeting.Slots.Single().Status);
    }

    private static ThesisProject CreateThesisProject()
    {
        return new ThesisProject(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Generative AI Thesis",
            "Evaluating transformer fine-tuning techniques");
    }

    private sealed class InMemoryMeetingRepository : IMeetingRepository
    {
        private readonly Dictionary<Guid, Meeting> _meetings = new();

        public Task AddAsync(Meeting meeting, CancellationToken cancellationToken = default)
        {
            _meetings[meeting.Id] = meeting;
            return Task.CompletedTask;
        }

        public Task<Meeting?> GetByIdAsync(Guid meetingId, CancellationToken cancellationToken = default)
        {
            _meetings.TryGetValue(meetingId, out var meeting);
            return Task.FromResult(meeting);
        }

        public Task<Meeting?> GetForUpdateAsync(Guid meetingId, CancellationToken cancellationToken = default)
        {
            _meetings.TryGetValue(meetingId, out var meeting);
            return Task.FromResult(meeting);
        }

        public Task<IReadOnlyCollection<Meeting>> GetUpcomingMeetingsForParticipantAsync(Guid participantId, DateTime fromUtc, CancellationToken cancellationToken = default)
        {
            var results = _meetings.Values
                .Where(meeting => meeting.StudentId == participantId || meeting.SupervisorId == participantId)
                .Where(meeting => meeting.Slots.Any(slot => slot.EndOn >= fromUtc))
                .ToArray();

            return Task.FromResult((IReadOnlyCollection<Meeting>)results);
        }

        public Task<bool> HasOverlappingMeetingAsync(Guid supervisorId, DateTime startUtc, DateTime endUtc, Guid? meetingIdToExclude = null, CancellationToken cancellationToken = default)
        {
            var hasOverlap = _meetings.Values
                .Where(meeting => meeting.SupervisorId == supervisorId && meeting.Status != MeetingStatuses.Cancelled)
                .Where(meeting => meetingIdToExclude is null || meeting.Id != meetingIdToExclude)
                .SelectMany(meeting => meeting.Slots)
                .Where(slot => slot.Status != MeetingSlotStatuses.Declined)
                .Any(slot => slot.StartOn < endUtc && startUtc < slot.EndOn);

            return Task.FromResult(hasOverlap);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class StubThesisProjectRepository : IThesisProjectRepository
    {
        private readonly Dictionary<Guid, ThesisProject> _theses = new();

        public Task AddAsync(ThesisProject thesis, CancellationToken cancellationToken = default)
        {
            _theses[thesis.Id] = thesis;
            return Task.CompletedTask;
        }

        public Task<ThesisProject?> GetByIdAsync(Guid thesisId, CancellationToken cancellationToken = default)
        {
            _theses.TryGetValue(thesisId, out var thesis);
            return Task.FromResult(thesis);
        }

        public Task<ThesisProject?> GetForUpdateAsync(Guid thesisId, CancellationToken cancellationToken = default)
        {
            _theses.TryGetValue(thesisId, out var thesis);
            return Task.FromResult(thesis);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}