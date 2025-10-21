using MediatR;
using University.Application.DTOs;

namespace University.Application.ThesisUpdates.Commands.SubmitThesisUpdate;

public record SubmitThesisUpdateCommand(
    Guid ThesisProjectId,
    string AuthorRole,
    string Notes,
    string? ArtifactUri
) : IRequest<ThesisUpdateDto>;