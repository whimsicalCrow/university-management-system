using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace University.Domain.Notifications;

public interface INotificationRepository
{
	Task<NotificationPreference?> GetPreferenceAsync(Guid userId, CancellationToken cancellationToken = default);

	Task UpsertPreferenceAsync(NotificationPreference preference, CancellationToken cancellationToken = default);

	Task AddNotificationAsync(NotificationRecord record, CancellationToken cancellationToken = default);

	Task<IReadOnlyCollection<NotificationRecord>> GetRecentNotificationsAsync(
		Guid userId,
		int maxCount,
		CancellationToken cancellationToken = default);
}
