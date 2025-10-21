using System;

namespace University.Application.DTOs;

public record StudentDto(
	Guid Id,
	string FirstName,
	string LastName,
	string Email,
	DateTime EnrollmentDate,
	Guid? SupervisorId
);
