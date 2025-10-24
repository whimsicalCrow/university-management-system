using FluentValidation;

namespace University.Application.Meetings.Commands.SetMeetingActionItemCompletion;

public sealed class SetMeetingActionItemCompletionCommandValidator : AbstractValidator<SetMeetingActionItemCompletionCommand>
{
    public SetMeetingActionItemCompletionCommandValidator()
    {
        RuleFor(command => command.MeetingId)
            .NotEmpty();

        RuleFor(command => command.ActionItemId)
            .NotEmpty();

        RuleFor(command => command.ActorId)
            .NotEmpty();
    }
}