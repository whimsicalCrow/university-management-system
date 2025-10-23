using System;
using System.Linq;
using MediatR;
using University.Domain.Aggregates.Theses;
using University.Domain.Repositories;

namespace University.Application.ThesisProjects.Queries.GetThesisUpdates;

public sealed class GetThesisUpdatesQueryHandler : IRequestHandler<GetThesisUpdatesQuery, IReadOnlyCollection<ThesisUpdateTimelineDto>>
{
    private readonly IThesisProjectRepository _repository;

    public GetThesisUpdatesQueryHandler(IThesisProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ThesisUpdateTimelineDto>> Handle(GetThesisUpdatesQuery request, CancellationToken cancellationToken)
    {
        var thesis = await _repository.GetByIdAsync(request.ThesisId, cancellationToken)
            .ConfigureAwait(false);

        if (thesis is null)
        {
            return Array.Empty<ThesisUpdateTimelineDto>();
        }

        var updates = thesis.Updates.AsEnumerable();

        if (!request.IncludeDrafts)
        {
            updates = updates.Where(update => !string.Equals(update.Status, ThesisUpdateStatuses.Draft, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            updates = updates.Where(update => string.Equals(update.Status, request.Status, StringComparison.OrdinalIgnoreCase));
        }

        return updates
            .OrderByDescending(update => update.OccurredOn)
            .Select(update => new ThesisUpdateTimelineDto(
                update.Id,
                update.AuthorId,
                update.Note,
                update.Status,
                update.OccurredOn,
                update.LastModifiedOn))
            .ToArray();
    }
}