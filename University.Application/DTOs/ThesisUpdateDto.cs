using System;

namespace University.Application.DTOs;

public record ThesisUpdateDto(
    Guid Id,
    Guid ThesisProjectId,
    string AuthorRole,
    string Notes,
    string? ArtifactUri,
    DateTime CreatedAtUtc,
    DateTime? ModifiedAtUtc
);