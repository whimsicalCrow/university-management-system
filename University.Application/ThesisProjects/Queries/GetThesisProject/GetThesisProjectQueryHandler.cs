using System.Linq;
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
                    update.Id,
                    update.AuthorId,
                    update.Note,
                    update.OccurredOn,
                    update.Status,
                    update.LastModifiedOn,
                    update.Comments
                        .Select(comment => new ThesisUpdateCommentDto(
                            comment.Id,
                            comment.AuthorId,
                            comment.Content,
                            comment.CreatedOn,
                            comment.LastEditedOn,
                            comment.ParentCommentId))
                        .ToArray(),
                    update.AuditTrail
                        .Select(entry => new ThesisUpdateAuditEntryDto(
                            entry.Id,
                            entry.ActorId,
                            entry.Action,
                            entry.Details,
                            entry.FromStatus,
                            entry.ToStatus,
                            entry.OccurredOn))
                        .ToArray(),
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
