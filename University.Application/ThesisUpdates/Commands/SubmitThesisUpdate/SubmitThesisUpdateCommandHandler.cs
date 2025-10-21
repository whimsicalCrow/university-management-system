using MediatR;
using University.Application.DTOs;
using University.Domain.Entities;
using University.Domain.Interfaces;

namespace University.Application.ThesisUpdates.Commands.SubmitThesisUpdate;

public class SubmitThesisUpdateCommandHandler : IRequestHandler<SubmitThesisUpdateCommand, ThesisUpdateDto>
{
    private readonly IThesisProjectRepository _thesisProjectRepository;
    private readonly IThesisUpdateRepository _thesisUpdateRepository;

    public SubmitThesisUpdateCommandHandler(
        IThesisProjectRepository thesisProjectRepository,
        IThesisUpdateRepository thesisUpdateRepository)
    {
        _thesisProjectRepository = thesisProjectRepository;
        _thesisUpdateRepository = thesisUpdateRepository;
    }

    public async Task<ThesisUpdateDto> Handle(SubmitThesisUpdateCommand request, CancellationToken cancellationToken)
    {
        var project = await _thesisProjectRepository.GetByIdAsync(request.ThesisProjectId, cancellationToken)
            ?? throw new InvalidOperationException($"Thesis project {request.ThesisProjectId} not found.");

        var update = new ThesisUpdate
        {
            ThesisProjectId = project.Id,
            AuthorRole = request.AuthorRole,
            Notes = request.Notes,
            ArtifactUri = request.ArtifactUri
        };

        update = await _thesisUpdateRepository.AddAsync(update, cancellationToken);

        project.LastUpdatedAtUtc = update.CreatedAtUtc;
        await _thesisProjectRepository.UpdateAsync(project, cancellationToken);

        return new ThesisUpdateDto(
            update.Id,
            update.ThesisProjectId,
            update.AuthorRole,
            update.Notes,
            update.ArtifactUri,
            update.CreatedAtUtc,
            update.ModifiedAtUtc
        );
    }
}