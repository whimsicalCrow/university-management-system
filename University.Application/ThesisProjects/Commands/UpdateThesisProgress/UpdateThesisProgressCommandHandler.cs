using MediatR;
using University.Domain.Aggregates.Theses;
using University.Domain.Repositories;

namespace University.Application.ThesisProjects.Commands.UpdateThesisProgress;

public sealed class UpdateThesisProgressCommandHandler : IRequestHandler<UpdateThesisProgressCommand, ThesisUpdateResultDto>
{
    private readonly IThesisProjectRepository _repository;

    public UpdateThesisProgressCommandHandler(IThesisProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThesisUpdateResultDto> Handle(UpdateThesisProgressCommand request, CancellationToken cancellationToken)
    {
        var thesis = await _repository.GetForUpdateAsync(request.ThesisId, cancellationToken)
            .ConfigureAwait(false);

        if (thesis is null)
        {
            throw new InvalidOperationException("The thesis project could not be found.");
        }

        if (thesis.StudentId != request.AuthorId)
        {
            throw new UnauthorizedAccessException("Only the thesis student can author progress updates.");
        }

        var status = request.SaveAsDraft ? ThesisUpdateStatuses.Draft : request.Status;

        if (!ThesisUpdateStatuses.IsValid(status))
        {
            throw new InvalidOperationException("Unknown status for thesis update.");
        }

        var update = thesis.SaveOrUpdateProgress(
            request.AuthorId,
            request.Content,
            request.OccurredOn,
            status,
            request.UpdateId);

        await _repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new ThesisUpdateResultDto(
            update.Id,
            update.Status,
            update.OccurredOn,
            update.LastModifiedOn);
    }
}