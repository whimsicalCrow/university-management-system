namespace University.Application.Commands;

using MediatR;
using Microsoft.Extensions.Logging;
using University.Application.Interfaces;
using University.Domain.Enums;
using University.Domain.Exceptions;

/// <summary>
/// Handles <see cref="SubmitReviewCommand"/>:
/// <list type="number">
///   <item>Resolves the professor and update entities.</item>
///   <item>Authorises the professor (must be assigned to the student who owns the update).</item>
///   <item>Optionally persists a <see cref="Domain.Entities.Feedback"/> row via the domain method.</item>
///   <item>Transitions the <see cref="Domain.Entities.ThesisUpdate"/> to the requested status.</item>
/// </list>
/// </summary>
public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, SubmitReviewResult>
{
    private readonly IThesisUpdateRepository _repository;
    private readonly ILogger<SubmitReviewCommandHandler> _logger;

    public SubmitReviewCommandHandler(
        IThesisUpdateRepository repository,
        ILogger<SubmitReviewCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SubmitReviewResult> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        // ── Guard: at least a comment or a non-default status change must be present ──
        var hasComment = !string.IsNullOrWhiteSpace(request.Comment);

        if (!hasComment &&
            !string.Equals(request.NewStatus, "Approved", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(request.NewStatus, "NeedsRevision", StringComparison.OrdinalIgnoreCase))
        {
            return SubmitReviewResult.Fail("A valid NewStatus (Approved or NeedsRevision) is required.");
        }

        // ── Resolve professor ──────────────────────────────────────────────────────
        var professor = await _repository.FindProfessorByEmailAsync(
            request.ProfessorUserName, cancellationToken);

        if (professor is null)
        {
            _logger.LogWarning(
                "SubmitReview rejected: no professor profile for user {UserName}",
                request.ProfessorUserName);
            return SubmitReviewResult.Fail("Professor profile not found for the authenticated user.");
        }

        // ── Resolve update ─────────────────────────────────────────────────────────
        var update = await _repository.GetByIdAsync(request.UpdateId, cancellationToken);

        if (update is null)
        {
            return SubmitReviewResult.Fail($"Thesis update #{request.UpdateId} not found.");
        }

        // ── Authorise: professor must be assigned to the student who owns this update ──
        var isAssigned = await _repository.IsProfessorAssignedToStudentAsync(
            professor.Id, update.StudentId, cancellationToken);

        if (!isAssigned)
        {
            _logger.LogWarning(
                "SubmitReview rejected: professor {ProfessorId} is not assigned to student {StudentId}",
                professor.Id, update.StudentId);
            return SubmitReviewResult.Fail("You are not authorised to review updates for this student.");
        }

        // ── Persist feedback comment (when provided) ───────────────────────────────
        if (hasComment)
        {
            try
            {
                var feedback = professor.ProvideFeedback(update.Id, request.Comment!.Trim());
                await _repository.AddFeedbackAsync(feedback, cancellationToken);
            }
            catch (DomainException ex)
            {
                return SubmitReviewResult.Fail(ex.Message);
            }
        }

        // ── Transition ThesisUpdate status ─────────────────────────────────────────
        try
        {
            if (string.Equals(request.NewStatus, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                update.MarkAsReviewed();
            }
            else if (string.Equals(request.NewStatus, "NeedsRevision", StringComparison.OrdinalIgnoreCase))
            {
                // RequestRevision sets status back to Submitted (see ThesisUpdate.RequestRevision for rationale)
                update.RequestRevision();
            }
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
                "SubmitReview status transition failed for update {UpdateId}: {Message}",
                request.UpdateId, ex.Message);
            // Non-fatal: feedback comment (if any) was already saved; log and continue
        }

        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Professor {ProfessorId} submitted review for update {UpdateId}: status={Status}, hasComment={HasComment}",
            professor.Id, update.Id, update.Status, hasComment);

        return SubmitReviewResult.Ok();
    }
}
