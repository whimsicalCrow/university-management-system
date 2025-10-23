using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using University.Application.DependencyInjection;
using University.Application.ThesisProjects.Commands.UpdateThesisProgress;
using University.Application.ThesisProjects.Queries.GetThesisUpdates;
using University.Domain.Aggregates.Theses;
using University.Domain.Repositories;
using University.Infrastructure.Persistence;
using University.Infrastructure.Repositories;

namespace University.IntegrationTests;

public class ThesisProgressIntegrationTests
{
    [Fact]
    public async Task Student_can_create_draft_and_publish_update()
    {
        await using var provider = BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var repository = provider.GetRequiredService<IThesisProjectRepository>();

        var studentId = Guid.NewGuid();
        var thesis = await SeedThesisAsync(repository, studentId);

        var draftResult = await mediator.Send(new UpdateThesisProgressCommand
        {
            ThesisId = thesis.Id,
            AuthorId = studentId,
            Content = "Σύντομη ενημέρωση για την εβδομάδα 1",
            OccurredOn = DateTime.UtcNow,
            Status = ThesisUpdateStatuses.Draft,
            SaveAsDraft = true,
        });

        Assert.Equal(ThesisUpdateStatuses.Draft, draftResult.Status);

        var publishedResult = await mediator.Send(new UpdateThesisProgressCommand
        {
            ThesisId = thesis.Id,
            AuthorId = studentId,
            UpdateId = draftResult.UpdateId,
            Content = "Ολοκληρώθηκε η ανάλυση δεδομένων για την εβδομάδα 1.",
            OccurredOn = DateTime.UtcNow,
            Status = ThesisUpdateStatuses.PendingReview,
        });

        Assert.Equal(ThesisUpdateStatuses.PendingReview, publishedResult.Status);

        var timeline = await mediator.Send(new GetThesisUpdatesQuery
        {
            ThesisId = thesis.Id,
            IncludeDrafts = true,
        });

        var entry = Assert.Single(timeline);
        Assert.Equal(publishedResult.UpdateId, entry.Id);
        Assert.Equal("Ολοκληρώθηκε η ανάλυση δεδομένων για την εβδομάδα 1.", entry.Note);
        Assert.Equal(ThesisUpdateStatuses.PendingReview, entry.Status);
    }

    [Fact]
    public async Task Command_rejects_unauthorized_author()
    {
        await using var provider = BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var repository = provider.GetRequiredService<IThesisProjectRepository>();

        var studentId = Guid.NewGuid();
        var thesis = await SeedThesisAsync(repository, studentId);

        var unauthorizedUser = Guid.NewGuid();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => mediator.Send(new UpdateThesisProgressCommand
        {
            ThesisId = thesis.Id,
            AuthorId = unauthorizedUser,
            Content = "Μη εξουσιοδοτημένη ενημέρωση",
            OccurredOn = DateTime.UtcNow,
            Status = ThesisUpdateStatuses.PendingReview,
        }));
    }

    [Fact]
    public async Task Timeline_filters_by_status_for_supervisor_view()
    {
        await using var provider = BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var repository = provider.GetRequiredService<IThesisProjectRepository>();

        var studentId = Guid.NewGuid();
        var thesis = await SeedThesisAsync(repository, studentId);

        await mediator.Send(new UpdateThesisProgressCommand
        {
            ThesisId = thesis.Id,
            AuthorId = studentId,
            Content = "Draft update",
            OccurredOn = DateTime.UtcNow.AddDays(-2),
            Status = ThesisUpdateStatuses.Draft,
            SaveAsDraft = true,
        });

        await mediator.Send(new UpdateThesisProgressCommand
        {
            ThesisId = thesis.Id,
            AuthorId = studentId,
            Content = "Αναμονή αξιολόγησης",
            OccurredOn = DateTime.UtcNow.AddDays(-1),
            Status = ThesisUpdateStatuses.PendingReview,
        });

        await mediator.Send(new UpdateThesisProgressCommand
        {
            ThesisId = thesis.Id,
            AuthorId = studentId,
            Content = "Εγκρίθηκε",
            OccurredOn = DateTime.UtcNow,
            Status = ThesisUpdateStatuses.Accepted,
        });

        var filtered = await mediator.Send(new GetThesisUpdatesQuery
        {
            ThesisId = thesis.Id,
            Status = ThesisUpdateStatuses.PendingReview,
            IncludeDrafts = false,
        });

        var entry = Assert.Single(filtered);
        Assert.Equal(ThesisUpdateStatuses.PendingReview, entry.Status);

        var ordered = filtered.OrderByDescending(item => item.OccurredOn).ToArray();
        Assert.Equal(filtered, ordered);
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddDbContext<UniversityDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddScoped<IThesisProjectRepository, ThesisProjectRepository>();

        services.AddApplicationLayer();

        return services.BuildServiceProvider();
    }

    private static async Task<ThesisProject> SeedThesisAsync(IThesisProjectRepository repository, Guid studentId)
    {
        var supervisorId = Guid.NewGuid();
        var thesis = new ThesisProject(
            studentId,
            supervisorId,
            "Διπλωματική εργασία",
            "Περιγραφή για δοκιμές αυτόματης ενημέρωσης.");

        await repository.AddAsync(thesis);
        return thesis;
    }
}
