using MediatR;
using University.Domain.Aggregates.Theses;

namespace University.Application.ThesisProjects.Commands.UpdateThesisProgress;

public sealed record UpdateThesisProgressCommand : IRequest<ThesisUpdateResultDto>
{
    public Guid ThesisId { get; init; }

    public Guid AuthorId { get; init; }

    public Guid? UpdateId { get; init; }

    public string Content { get; init; } = string.Empty;

    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    public string Status { get; init; } = ThesisUpdateStatuses.PendingReview;

    public bool SaveAsDraft { get; init; }
        = false;
}

public sealed record ThesisUpdateResultDto(
    Guid UpdateId,
    string Status,
    DateTime OccurredOn,
    DateTime LastModifiedOn);