using MediatR;
using University.Application.DTOs;

namespace University.Application.ThesisProjects.Queries.GetWorkspaceOverview;

public record GetWorkspaceOverviewQuery(Guid ThesisProjectId) : IRequest<ThesisWorkspaceOverviewDto>;