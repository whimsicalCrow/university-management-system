using FluentValidation;

namespace University.Application.ThesisProjects.Commands.CreateThesisProject;

public class CreateThesisProjectCommandValidator : AbstractValidator<CreateThesisProjectCommand>
{
    public CreateThesisProjectCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.ProfessorId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}