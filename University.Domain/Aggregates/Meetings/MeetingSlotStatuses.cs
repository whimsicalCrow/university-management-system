using System.Collections.Generic;

namespace University.Domain.Aggregates.Meetings;

public static class MeetingSlotStatuses
{
    public const string Proposed = "Proposed";
    public const string Accepted = "Accepted";
    public const string Declined = "Declined";

    private static readonly HashSet<string> Allowed = new()
    {
        Proposed,
        Accepted,
        Declined,
    };

    public static IReadOnlyCollection<string> All => Allowed;

    public static bool IsValid(string? status)
    {
        return !string.IsNullOrWhiteSpace(status) && Allowed.Contains(status);
    }
}