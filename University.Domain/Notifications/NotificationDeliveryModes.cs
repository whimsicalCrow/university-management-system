using System.Collections.Generic;

namespace University.Domain.Notifications;

public static class NotificationDeliveryModes
{
	public const string InApp = "InApp";
	public const string Email = "Email";

	private static readonly HashSet<string> _allowed = new(new[] { InApp, Email });

	public static IReadOnlyCollection<string> All => _allowed;

	public static bool IsValid(string? mode)
	{
		return !string.IsNullOrWhiteSpace(mode) && _allowed.Contains(mode);
	}
}
