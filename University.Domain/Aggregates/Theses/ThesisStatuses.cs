namespace University.Domain.Aggregates.Theses;

public static class ThesisStatuses
{
    public const string Draft = "Draft";
    public const string InReview = "InReview";
    public const string Approved = "Approved";
    public const string NeedsChanges = "NeedsChanges";

    private static readonly HashSet<string> _all = new(StringComparer.OrdinalIgnoreCase)
    {
        Draft,
        InReview,
        Approved,
        NeedsChanges
    };

    public static IReadOnlySet<string> All => _all;
}
