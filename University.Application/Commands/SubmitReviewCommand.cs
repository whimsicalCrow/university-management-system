namespace University.Application.Commands;

using MediatR;

/// <summary>
/// Submits a professor review for a thesis update: optionally records a comment (Feedback row)
/// and transitions the <see cref="Domain.Entities.ThesisUpdate"/> to the requested status.
/// </summary>
public class SubmitReviewCommand : IRequest<SubmitReviewResult>
{
    /// <summary>ASP.NET Core Identity email (username) of the authenticated professor.</summary>
    public required string ProfessorUserName { get; set; }

    /// <summary>Primary key of the <see cref="Domain.Entities.ThesisUpdate"/> being reviewed.</summary>
    public required int UpdateId { get; set; }

    /// <summary>
    /// Optional feedback comment. When provided a <see cref="Domain.Entities.Feedback"/> record
    /// is persisted via <see cref="Domain.Entities.Professor.ProvideFeedback"/>.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Desired outcome status. Accepted values:
    /// <list type="bullet">
    ///   <item><term>Approved</term><description>Marks the update as Reviewed.</description></item>
    ///   <item><term>NeedsRevision</term><description>Sets the update back to Submitted so the student can revise.</description></item>
    /// </list>
    /// </summary>
    public required string NewStatus { get; set; }
}

/// <summary>Result of <see cref="SubmitReviewCommand"/>.</summary>
public class SubmitReviewResult
{
    public bool Success { get; private set; }
    public string? Error { get; private set; }

    public static SubmitReviewResult Ok() => new() { Success = true };
    public static SubmitReviewResult Fail(string error) => new() { Success = false, Error = error };
}
