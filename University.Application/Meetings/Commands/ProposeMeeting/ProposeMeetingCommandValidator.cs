using FluentValidation;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.ProposeMeeting;

public sealed class ProposeMeetingCommandValidator : AbstractValidator<ProposeMeetingCommand>
{
    public ProposeMeetingCommandValidator()
    {
        RuleFor(command => command.ThesisProjectId)
            .NotEmpty();

        RuleFor(command => command.StudentId)
            .NotEmpty();

        RuleFor(command => command.Agenda)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(command => command.VideoConferenceUrl)
            .MaximumLength(512)
            .When(command => !string.IsNullOrWhiteSpace(command.VideoConferenceUrl));

        RuleFor(command => command.Slots)
            .NotNull()
            .Must(slots => slots.Count > 0)
            .WithMessage("At least one meeting slot must be provided.");

        RuleForEach(command => command.Slots)
            .SetValidator(new ProposedMeetingSlotDtoValidator());
    }

    private sealed class ProposedMeetingSlotDtoValidator : AbstractValidator<ProposedMeetingSlotDto>
    {
        public ProposedMeetingSlotDtoValidator()
        {
            RuleFor(slot => slot.StartOn)
                .NotEmpty();

            RuleFor(slot => slot.EndOn)
                .NotEmpty()
                .GreaterThan(slot => slot.StartOn);

            RuleFor(slot => slot.Note)
                .MaximumLength(1000)
                .When(slot => !string.IsNullOrWhiteSpace(slot.Note));
        }
    }
}