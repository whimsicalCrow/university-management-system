using MediatR;
using University.Application.DTOs;
using University.Domain.Entities;
using University.Domain.Interfaces;

namespace University.Application.Commands;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, StudentDto>
{
    private readonly IStudentRepository _studentRepository;

    public CreateStudentCommandHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = new Student
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            EnrollmentDate = request.EnrollmentDate,
            SupervisorId = request.SupervisorId
        };

        student = await _studentRepository.AddAsync(student, cancellationToken);

        return new StudentDto(
            student.Id,
            student.FirstName,
            student.LastName,
            student.Email,
            student.EnrollmentDate,
            student.SupervisorId
        );
    }
}