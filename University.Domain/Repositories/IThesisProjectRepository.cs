using University.Domain.Aggregates.Theses;

namespace University.Domain.Repositories;

public interface IThesisProjectRepository
{
    Task<ThesisProject?> GetByIdAsync(Guid thesisId, CancellationToken cancellationToken = default);

    Task<ThesisProject?> GetForUpdateAsync(Guid thesisId, CancellationToken cancellationToken = default);

    Task AddAsync(ThesisProject thesis, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
