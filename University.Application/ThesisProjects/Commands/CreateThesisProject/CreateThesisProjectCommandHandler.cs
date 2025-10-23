using MediatR;
using University.Domain.Aggregates.Theses;
using University.Domain.Repositories;

namespace University.Application.ThesisProjects.Commands.CreateThesisProject;

public sealed class CreateThesisProjectCommandHandler : IRequestHandler<CreateThesisProjectCommand, Guid>
{
    private readonly IThesisProjectRepository _repository;

    public CreateThesisProjectCommandHandler(IThesisProjectRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateThesisProjectCommand request, CancellationToken cancellationToken)
    {
        var thesis = new ThesisProject(request.StudentId, request.SupervisorId, request.Title, request.Summary);

        await _repository.AddAsync(thesis, cancellationToken).ConfigureAwait(false);

        return thesis.Id;
    }
}
