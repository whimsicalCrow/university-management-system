using MediatR;
using University.Application.DTOs;

namespace University.Application.Commands;

public record CreateStudentCommand(
	string FirstName,
	string LastName,
	string Email,
	DateTime EnrollmentDate,
	Guid? SupervisorId
) : IRequest<StudentDto>;
