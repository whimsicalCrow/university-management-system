using FluentValidation;
using University.Domain.Aggregates.Theses;

namespace University.Application.ThesisProjects.Queries.GetThesisUpdates;

public sealed class GetThesisUpdatesQueryValidator : AbstractValidator<GetThesisUpdatesQuery>
{
    public GetThesisUpdatesQueryValidator()
    {
        RuleFor(query => query.ThesisId)
            .NotEmpty();

        RuleFor(query => query.Status)
            .Must(status => string.IsNullOrWhiteSpace(status) || ThesisUpdateStatuses.IsValid(status))
            .WithMessage("Παρακαλούμε επιλέξτε μια έγκυρη κατάσταση φίλτρου.");
    }
}