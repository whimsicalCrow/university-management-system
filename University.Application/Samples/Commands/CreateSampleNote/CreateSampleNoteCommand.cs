using FluentValidation;
using MediatR;
using University.Application.Samples.Shared;

namespace University.Application.Samples.Commands.CreateSampleNote;

public sealed record CreateSampleNoteCommand(string Title, string? Summary) : IRequest<SampleNoteDto>;

public sealed class CreateSampleNoteCommandHandler : IRequestHandler<CreateSampleNoteCommand, SampleNoteDto>
{
    public Task<SampleNoteDto> Handle(CreateSampleNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new SampleNoteDto(Guid.NewGuid(), request.Title.Trim(), request.Summary?.Trim() ?? string.Empty, DateTime.UtcNow);
        return Task.FromResult(note);
    }
}

public sealed class CreateSampleNoteCommandValidator : AbstractValidator<CreateSampleNoteCommand>
{
    public CreateSampleNoteCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.Summary)
            .MaximumLength(4000);
    }
}