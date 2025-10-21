using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using University.Domain.Entities;

namespace University.Domain.Interfaces;

public interface IThesisProjectRepository
{
    Task<ThesisProject> AddAsync(ThesisProject project, CancellationToken cancellationToken = default);
    Task<ThesisProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ThesisProject>> GetByProfessorAsync(Guid professorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ThesisProject>> GetByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task UpdateAsync(ThesisProject project, CancellationToken cancellationToken = default);
}