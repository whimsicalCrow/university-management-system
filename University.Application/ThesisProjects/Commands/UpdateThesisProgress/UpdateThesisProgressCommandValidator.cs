using FluentValidation;
using University.Domain.Aggregates.Theses;

namespace University.Application.ThesisProjects.Commands.UpdateThesisProgress;

public sealed class UpdateThesisProgressCommandValidator : AbstractValidator<UpdateThesisProgressCommand>
{
    private const int MaxContentLength = 4000;

    public UpdateThesisProgressCommandValidator()
    {
        RuleFor(command => command.ThesisId)
            .NotEmpty();

        RuleFor(command => command.AuthorId)
            .NotEmpty();

        RuleFor(command => command.Content)
            .NotEmpty().WithMessage("Παρακαλούμε γράψτε μια ενημέρωση πριν αποθηκεύσετε.")
            .MaximumLength(MaxContentLength).WithMessage($"Η ενημέρωση δεν μπορεί να ξεπερνά τους {MaxContentLength} χαρακτήρες.");

        RuleFor(command => command.Status)
            .NotEmpty().When(command => !command.SaveAsDraft)
            .Must(ThesisUpdateStatuses.IsValid)
            .When(command => !command.SaveAsDraft)
            .WithMessage("Επιλέξτε μία έγκυρη κατάσταση ενημέρωσης.");

        RuleFor(command => command.Status)
            .Equal(ThesisUpdateStatuses.Draft)
            .When(command => command.SaveAsDraft)
            .WithMessage("Τα πρόχειρα αποθηκεύονται πάντα ως Draft.");
    }
}