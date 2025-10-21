using FluentValidation;

namespace University.Application.ThesisUpdates.Commands.SubmitThesisUpdate;

public class SubmitThesisUpdateCommandValidator : AbstractValidator<SubmitThesisUpdateCommand>
{
    public SubmitThesisUpdateCommandValidator()
    {
        RuleFor(x => x.ThesisProjectId).NotEmpty();
        RuleFor(x => x.AuthorRole).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Notes).NotEmpty().MaximumLength(6000);
        RuleFor(x => x.ArtifactUri).MaximumLength(2048);
    }
}