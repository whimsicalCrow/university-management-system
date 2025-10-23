using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using University.Domain.Notifications;
using University.Infrastructure.Persistence;

namespace University.Infrastructure.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
	private readonly UniversityDbContext _dbContext;

	public NotificationRepository(UniversityDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<NotificationPreference?> GetPreferenceAsync(Guid userId, CancellationToken cancellationToken = default)
	{
		return await _dbContext
			.NotificationPreferences
			.AsNoTracking()
			.FirstOrDefaultAsync(preference => preference.UserId == userId, cancellationToken)
			.ConfigureAwait(false);
	}

    public async Task UpsertPreferenceAsync(NotificationPreference preference, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext
            .NotificationPreferences
            .FirstOrDefaultAsync(entity => entity.UserId == preference.UserId, cancellationToken)
            .ConfigureAwait(false);

        if (existing is null)
        {
            await _dbContext.NotificationPreferences.AddAsync(preference, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            existing.UpdateDeliveryMode(preference.DeliveryMode);
        }

        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddNotificationAsync(NotificationRecord record, CancellationToken cancellationToken = default)
    {
        await _dbContext.NotificationRecords.AddAsync(record, cancellationToken).ConfigureAwait(false);
        await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<NotificationRecord>> GetRecentNotificationsAsync(
        Guid userId,
        int maxCount,
        CancellationToken cancellationToken = default)
    {
        IQueryable<NotificationRecord> query = _dbContext
            .NotificationRecords
            .AsNoTracking()
            .Where(record => record.UserId == userId);

        query = query.OrderByDescending(record => record.CreatedOn);

        if (maxCount > 0)
        {
            query = query.Take(maxCount);
        }

        var notifications = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        return notifications;
    }
}
