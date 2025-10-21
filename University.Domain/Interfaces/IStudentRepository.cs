using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using University.Domain.Entities;

namespace University.Domain.Interfaces;

public interface IStudentRepository
{
    Task<Student> AddAsync(Student student, CancellationToken cancellationToken = default);
    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetBySupervisorAsync(Guid professorId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
}