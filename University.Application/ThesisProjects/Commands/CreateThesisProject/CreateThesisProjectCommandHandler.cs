using MediatR;
using University.Application.DTOs;
using University.Domain.Entities;
using University.Domain.Interfaces;

namespace University.Application.ThesisProjects.Commands.CreateThesisProject;

public class CreateThesisProjectCommandHandler : IRequestHandler<CreateThesisProjectCommand, ThesisProjectDto>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IProfessorRepository _professorRepository;
    private readonly IThesisProjectRepository _thesisProjectRepository;

    public CreateThesisProjectCommandHandler(
        IStudentRepository studentRepository,
        IProfessorRepository professorRepository,
        IThesisProjectRepository thesisProjectRepository)
    {
        _studentRepository = studentRepository;
        _professorRepository = professorRepository;
        _thesisProjectRepository = thesisProjectRepository;
    }

    public async Task<ThesisProjectDto> Handle(CreateThesisProjectCommand request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new InvalidOperationException($"Student {request.StudentId} not found.");

        var professor = await _professorRepository.GetByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new InvalidOperationException($"Professor {request.ProfessorId} not found.");

        student.SupervisorId = professor.Id;
        await _studentRepository.UpdateAsync(student, cancellationToken);

        var project = new ThesisProject
        {
            StudentId = student.Id,
            ProfessorId = professor.Id,
            Title = request.Title,
            Description = request.Description,
            Status = ThesisProjectStatus.InProgress
        };

        project = await _thesisProjectRepository.AddAsync(project, cancellationToken);

        return new ThesisProjectDto(
            project.Id,
            project.Title,
            project.Description,
            project.StudentId,
            project.ProfessorId,
            project.Status.ToString(),
            project.CreatedAtUtc,
            project.LastUpdatedAtUtc
        );
    }
}