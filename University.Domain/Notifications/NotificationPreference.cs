using System;
using University.Domain.Common;

namespace University.Domain.Notifications;

public sealed class NotificationPreference : Entity
{
	public NotificationPreference(Guid userId, string deliveryMode)
		: base(Guid.NewGuid())
	{
		if (userId == Guid.Empty)
		{
			throw new ArgumentException("User id must be provided", nameof(userId));
		}

		if (!NotificationDeliveryModes.IsValid(deliveryMode))
		{
			throw new ArgumentException("Unknown delivery mode", nameof(deliveryMode));
		}

		UserId = userId;
		DeliveryMode = deliveryMode;
		UpdatedOn = DateTime.UtcNow;
	}

	private NotificationPreference()
	{
		DeliveryMode = NotificationDeliveryModes.InApp;
		UpdatedOn = DateTime.UtcNow;
	}

	public Guid UserId { get; private set; }

	public string DeliveryMode { get; private set; }

	public DateTime UpdatedOn { get; private set; }

	public void UpdateDeliveryMode(string deliveryMode)
	{
		if (!NotificationDeliveryModes.IsValid(deliveryMode))
		{
			throw new ArgumentException("Unknown delivery mode", nameof(deliveryMode));
		}

		DeliveryMode = deliveryMode;
		UpdatedOn = DateTime.UtcNow;
	}
}
