using System;
using University.Domain.Common;

namespace University.Domain.Notifications;

public sealed class NotificationRecord : Entity
{
	public NotificationRecord(Guid userId, string title, string message, string? reference = null)
		: base(Guid.NewGuid())
	{
		if (userId == Guid.Empty)
		{
			throw new ArgumentException("User id must be provided", nameof(userId));
		}

		if (string.IsNullOrWhiteSpace(title))
		{
			throw new ArgumentException("Title must be provided", nameof(title));
		}

		if (string.IsNullOrWhiteSpace(message))
		{
			throw new ArgumentException("Message must be provided", nameof(message));
		}

		UserId = userId;
		Title = title;
		Message = message;
		Reference = reference;
		CreatedOn = DateTime.UtcNow;
	}

	private NotificationRecord()
	{
		Title = string.Empty;
		Message = string.Empty;
		CreatedOn = DateTime.UtcNow;
	}

	public Guid UserId { get; private set; }

	public string Title { get; private set; }

	public string Message { get; private set; }

	public string? Reference { get; private set; }

	public bool IsRead { get; private set; }

	public DateTime CreatedOn { get; private set; }

	public DateTime? ReadOn { get; private set; }

	public void AttachReference(string? reference)
	{
		Reference = reference;
	}

	public void MarkAsRead()
	{
		if (IsRead)
		{
			return;
		}

		IsRead = true;
		ReadOn = DateTime.UtcNow;
	}
}
