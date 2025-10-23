using MediatR;

namespace University.Application.ThesisProjects.Queries.GetThesisUpdates;

public sealed record GetThesisUpdatesQuery : IRequest<IReadOnlyCollection<ThesisUpdateTimelineDto>>
{
    public Guid ThesisId { get; init; }

    public string? Status { get; init; }

    public bool IncludeDrafts { get; init; } = false;
}

public sealed record ThesisUpdateTimelineDto(
    Guid Id,
    Guid AuthorId,
    string Note,
    string Status,
    DateTime OccurredOn,
    DateTime LastModifiedOn);