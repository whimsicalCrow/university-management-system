using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using University.Domain.Entities;

namespace University.Domain.Interfaces;

public interface IProfessorRepository
{
    Task<Professor> AddAsync(Professor professor, CancellationToken cancellationToken = default);
    Task<Professor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Professor>> ListAsync(CancellationToken cancellationToken = default);
}