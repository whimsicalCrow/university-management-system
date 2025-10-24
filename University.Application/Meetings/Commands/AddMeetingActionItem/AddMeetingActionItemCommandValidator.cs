using FluentValidation;

namespace University.Application.Meetings.Commands.AddMeetingActionItem;

public sealed class AddMeetingActionItemCommandValidator : AbstractValidator<AddMeetingActionItemCommand>
{
    public AddMeetingActionItemCommandValidator()
    {
        RuleFor(command => command.MeetingId)
            .NotEmpty();

        RuleFor(command => command.ActorId)
            .NotEmpty();

        RuleFor(command => command.OwnerId)
            .NotEmpty();

        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(2000);
    }
}