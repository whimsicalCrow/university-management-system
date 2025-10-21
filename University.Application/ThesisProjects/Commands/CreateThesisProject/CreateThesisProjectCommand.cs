using MediatR;
using University.Application.DTOs;

namespace University.Application.ThesisProjects.Commands.CreateThesisProject;

public record CreateThesisProjectCommand(
    Guid StudentId,
    Guid ProfessorId,
    string Title,
    string Description
) : IRequest<ThesisProjectDto>;