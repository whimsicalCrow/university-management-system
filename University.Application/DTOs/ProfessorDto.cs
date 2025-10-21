using System;

namespace University.Application.DTOs;

public record ProfessorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Department
);