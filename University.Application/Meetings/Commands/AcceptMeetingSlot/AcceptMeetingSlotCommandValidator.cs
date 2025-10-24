using FluentValidation;

namespace University.Application.Meetings.Commands.AcceptMeetingSlot;

public sealed class AcceptMeetingSlotCommandValidator : AbstractValidator<AcceptMeetingSlotCommand>
{
    public AcceptMeetingSlotCommandValidator()
    {
        RuleFor(command => command.MeetingId)
            .NotEmpty();

        RuleFor(command => command.SlotId)
            .NotEmpty();

        RuleFor(command => command.ProfessorId)
            .NotEmpty();

        RuleFor(command => command.VideoConferenceUrl)
            .MaximumLength(512)
            .When(command => !string.IsNullOrWhiteSpace(command.VideoConferenceUrl));
    }
}