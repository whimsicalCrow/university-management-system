using MediatR;
using University.Application.DTOs;

namespace University.Application.Queries;

public record GetStudentByIdQuery(Guid Id) : IRequest<StudentDto?>;
