using MediatR;

namespace University.Application.ThesisProjects.Queries.GetThesisProject;

public sealed record GetThesisProjectQuery(Guid ThesisId) : IRequest<ThesisProjectDto?>;
