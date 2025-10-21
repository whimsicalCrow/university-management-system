using System;

namespace University.Application.DTOs;

public record ThesisProjectDto(
    Guid Id,
    string Title,
    string Description,
    Guid StudentId,
    Guid ProfessorId,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? LastUpdatedAtUtc
);