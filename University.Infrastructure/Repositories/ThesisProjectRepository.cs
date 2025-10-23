using Microsoft.EntityFrameworkCore;
using University.Domain.Aggregates.Theses;
using University.Domain.Repositories;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public sealed class ThesisProjectRepository : IThesisProjectRepository
{
    private readonly UniversityDbContext _dbContext;

    public ThesisProjectRepository(UniversityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ThesisProject?> GetByIdAsync(Guid thesisId, CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .ThesisProjects
            .AsNoTracking()
            .Include(thesis => thesis.Updates)
            .ThenInclude(update => update.Attachments)
            .FirstOrDefaultAsync(thesis => thesis.Id == thesisId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(ThesisProject thesis, CancellationToken cancellationToken = default)
    {
        await _dbContext.ThesisProjects.AddAsync(thesis, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
