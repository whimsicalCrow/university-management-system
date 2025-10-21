using FluentValidation;

namespace University.Application.Meetings.Commands.ScheduleMeeting;

public class ScheduleMeetingCommandValidator : AbstractValidator<ScheduleMeetingCommand>
{
    public ScheduleMeetingCommandValidator()
    {
        RuleFor(x => x.ThesisProjectId).NotEmpty();
        RuleFor(x => x.ScheduledForUtc)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Meetings must be scheduled in the future.");
        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.FromMinutes(0))
            .LessThanOrEqualTo(TimeSpan.FromHours(4));
        RuleFor(x => x.Location).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Agenda).NotEmpty().MaximumLength(2000);
    }
}