using MediatR;
using University.Domain.Repositories;

namespace University.Application.ThesisProjects.Queries.GetThesisProject;

public sealed class GetThesisProjectQueryHandler : IRequestHandler<GetThesisProjectQuery, ThesisProjectDto?>
{
    private readonly IThesisProjectRepository _repository;

    public GetThesisProjectQueryHandler(IThesisProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<ThesisProjectDto?> Handle(GetThesisProjectQuery request, CancellationToken cancellationToken)
    {
        var thesis = await _repository.GetByIdAsync(request.ThesisId, cancellationToken).ConfigureAwait(false);

        if (thesis is null)
        {
            return null;
        }

        return new ThesisProjectDto(
            thesis.Id,
            thesis.StudentId,
            thesis.SupervisorId,
            thesis.Title,
            thesis.Summary,
            thesis.Status,
            thesis.Updates
                .Select(update => new ThesisUpdateDto(
                    update.AuthorId,
                    update.Note,
                    update.OccurredOn,
                    update.Attachments
                        .Select(attachment => new ThesisAttachmentDto(
                            attachment.FileName,
                            attachment.ContentType,
                            attachment.BlobName,
                            attachment.SizeBytes))
                        .ToArray()))
                .ToArray());
    }
}
