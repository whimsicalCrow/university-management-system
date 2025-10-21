using MediatR;
using University.Application.DTOs;
using University.Domain.Interfaces;

namespace University.Application.Queries;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentDto?>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentByIdQueryHandler(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentDto?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (student is null)
        {
            return null;
        }

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