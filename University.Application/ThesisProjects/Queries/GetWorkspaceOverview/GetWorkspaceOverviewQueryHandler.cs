using System.Linq;
using MediatR;
using University.Application.DTOs;
using University.Domain.Interfaces;

namespace University.Application.ThesisProjects.Queries.GetWorkspaceOverview;

public class GetWorkspaceOverviewQueryHandler : IRequestHandler<GetWorkspaceOverviewQuery, ThesisWorkspaceOverviewDto>
{
    private readonly IThesisProjectRepository _thesisProjectRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IThesisUpdateRepository _thesisUpdateRepository;
    private readonly IMeetingRepository _meetingRepository;

    public GetWorkspaceOverviewQueryHandler(
        IThesisProjectRepository thesisProjectRepository,
        IStudentRepository studentRepository,
        IProfessorRepository professorRepository,
        IThesisUpdateRepository thesisUpdateRepository,
        IMeetingRepository meetingRepository)
    {
        _thesisProjectRepository = thesisProjectRepository;
        _studentRepository = studentRepository;
        _professorRepository = professorRepository;
        _thesisUpdateRepository = thesisUpdateRepository;
        _meetingRepository = meetingRepository;
    }

    public async Task<ThesisWorkspaceOverviewDto> Handle(GetWorkspaceOverviewQuery request, CancellationToken cancellationToken)
    {
        var project = await _thesisProjectRepository.GetByIdAsync(request.ThesisProjectId, cancellationToken)
            ?? throw new InvalidOperationException($"Thesis project {request.ThesisProjectId} not found.");

        var student = await _studentRepository.GetByIdAsync(project.StudentId, cancellationToken)
            ?? throw new InvalidOperationException($"Student {project.StudentId} not found.");

        var professor = await _professorRepository.GetByIdAsync(project.ProfessorId, cancellationToken)
            ?? throw new InvalidOperationException($"Professor {project.ProfessorId} not found.");

        var updates = await _thesisUpdateRepository.GetByThesisIdAsync(project.Id, cancellationToken);
        var meetings = await _meetingRepository.GetByThesisIdAsync(project.Id, cancellationToken);

        var projectDto = new ThesisProjectDto(
            project.Id,
            project.Title,
            project.Description,
            project.StudentId,
            project.ProfessorId,
            project.Status.ToString(),
            project.CreatedAtUtc,
            project.LastUpdatedAtUtc
        );

        var studentDto = new StudentDto(
            student.Id,
            student.FirstName,
            student.LastName,
            student.Email,
            student.EnrollmentDate,
            student.SupervisorId
        );

        var professorDto = new ProfessorDto(
            professor.Id,
            professor.FirstName,
            professor.LastName,
            professor.Email,
            professor.Department
        );

        var updateDtos = updates
            .Select(u => new ThesisUpdateDto(u.Id, u.ThesisProjectId, u.AuthorRole, u.Notes, u.ArtifactUri, u.CreatedAtUtc, u.ModifiedAtUtc))
            .OrderByDescending(u => u.CreatedAtUtc)
            .ToList();

        var meetingDtos = meetings
            .Select(m => new MeetingDto(m.Id, m.ThesisProjectId, m.ScheduledForUtc, m.Duration, m.Location, m.Agenda, m.Notes, m.Status.ToString()))
            .OrderByDescending(m => m.ScheduledForUtc)
            .ToList();

        return new ThesisWorkspaceOverviewDto(
            projectDto,
            studentDto,
            professorDto,
            updateDtos,
            meetingDtos
        );
    }
}