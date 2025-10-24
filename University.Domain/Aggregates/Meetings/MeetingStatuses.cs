using System.Collections.Generic;

namespace University.Domain.Aggregates.Meetings;

public static class MeetingStatuses
{
    public const string Proposed = "Proposed";
    public const string Confirmed = "Confirmed";
    public const string Declined = "Declined";
    public const string Cancelled = "Cancelled";

    private static readonly HashSet<string> Allowed = new()
    {
        Proposed,
        Confirmed,
        Declined,
        Cancelled,
    };

    public static IReadOnlyCollection<string> All => Allowed;

    public static bool IsValid(string? status)
    {
        return !string.IsNullOrWhiteSpace(status) && Allowed.Contains(status);
    }
}