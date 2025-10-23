using FluentValidation;

namespace University.Application.ThesisProjects.Commands.CreateThesisProject;

public sealed class CreateThesisProjectCommandValidator : AbstractValidator<CreateThesisProjectCommand>
{
    public CreateThesisProjectCommandValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty();

        RuleFor(x => x.SupervisorId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
