using FluentValidation;

namespace University.Application.Meetings.Commands.UpdateMeetingActionItem;

public sealed class UpdateMeetingActionItemCommandValidator : AbstractValidator<UpdateMeetingActionItemCommand>
{
    public UpdateMeetingActionItemCommandValidator()
    {
        RuleFor(command => command.MeetingId)
            .NotEmpty();

        RuleFor(command => command.ActionItemId)
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