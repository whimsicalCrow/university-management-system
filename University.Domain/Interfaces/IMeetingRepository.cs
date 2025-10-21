using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using University.Domain.Entities;

namespace University.Domain.Interfaces;

public interface IMeetingRepository
{
    Task<Meeting> AddAsync(Meeting meeting, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Meeting>> GetByThesisIdAsync(Guid thesisProjectId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Meeting meeting, CancellationToken cancellationToken = default);
}