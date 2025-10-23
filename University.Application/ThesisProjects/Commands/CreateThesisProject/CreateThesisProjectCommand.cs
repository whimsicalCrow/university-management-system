using MediatR;

namespace University.Application.ThesisProjects.Commands.CreateThesisProject;

public sealed record CreateThesisProjectCommand(
    Guid StudentId,
    Guid SupervisorId,
    string Title,
    string Summary) : IRequest<Guid>;
