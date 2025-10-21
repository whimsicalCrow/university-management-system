using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using University.Domain.Entities;

namespace University.Domain.Interfaces;

public interface IThesisUpdateRepository
{
    Task<ThesisUpdate> AddAsync(ThesisUpdate update, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ThesisUpdate>> GetByThesisIdAsync(Guid thesisProjectId, CancellationToken cancellationToken = default);
}