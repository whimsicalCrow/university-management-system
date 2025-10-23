using System;
using System.Collections.Generic;

namespace University.Domain.Aggregates.Theses;

public static class ThesisUpdateStatuses
{
    private static readonly HashSet<string> StatusSet = new(StringComparer.OrdinalIgnoreCase)
    {
        Draft,
        PendingReview,
        Accepted,
        NeedsRevision,
    };

    public const string Draft = "Draft";
    public const string PendingReview = "Pending Review";
    public const string Accepted = "Accepted";
    public const string NeedsRevision = "Needs Revision";

    public static readonly IReadOnlyCollection<string> All = StatusSet;

    public static bool IsValid(string status) => !string.IsNullOrWhiteSpace(status) && StatusSet.Contains(status);
}