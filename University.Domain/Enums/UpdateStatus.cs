namespace University.Domain.Enums;

/// <summary>
/// Represents the status of a thesis update submission.
/// </summary>
public enum UpdateStatus
{
    /// <summary>
    /// The update is in draft state and not yet submitted.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// The update has been submitted for review.
    /// </summary>
    Submitted = 1,

    /// <summary>
    /// The update has been reviewed.
    /// </summary>
    Reviewed = 2
}
