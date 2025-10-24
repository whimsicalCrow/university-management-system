using System.Collections.Generic;

namespace University.Domain.Aggregates.Meetings;

public static class MeetingActionItemStatuses
{
    public const string Pending = "Pending";
    public const string Completed = "Completed";

    private static readonly HashSet<string> Allowed = new()
    {
        Pending,
        Completed,
    };

    public static IReadOnlyCollection<string> All => Allowed;

    public static bool IsValid(string? status)
    {
        return !string.IsNullOrWhiteSpace(status) && Allowed.Contains(status);
    }
}