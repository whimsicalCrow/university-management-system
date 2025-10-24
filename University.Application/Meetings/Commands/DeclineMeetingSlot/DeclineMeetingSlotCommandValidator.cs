using FluentValidation;
using University.Application.Meetings.Shared;

namespace University.Application.Meetings.Commands.DeclineMeetingSlot;

public sealed class DeclineMeetingSlotCommandValidator : AbstractValidator<DeclineMeetingSlotCommand>
{
    public DeclineMeetingSlotCommandValidator()
    {
        RuleFor(command => command.MeetingId)
            .NotEmpty();

        RuleFor(command => command.SlotId)
            .NotEmpty();

        RuleFor(command => command.ProfessorId)
            .NotEmpty();

        RuleFor(command => command.ResponseNote)
            .MaximumLength(1000)
            .When(command => !string.IsNullOrWhiteSpace(command.ResponseNote));

        When(command => command.AlternativeSlot is not null, () =>
        {
            RuleFor(command => command.AlternativeSlot!)
                .SetValidator(new ProposedMeetingSlotDtoValidator());
        });
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