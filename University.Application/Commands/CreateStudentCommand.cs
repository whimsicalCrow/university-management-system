using MediatR;
using University.Application.DTOs;

namespace University.Application.Commands;

public record CreateStudentCommand(string FirstName, string LastName) : IRequest<StudentDto>;
