using MediatR;
using University.Application.Samples.Shared;

namespace University.Application.Samples.Queries.GetSampleNotes;

public sealed record GetSampleNotesQuery : IRequest<IReadOnlyCollection<SampleNoteDto>>;

public sealed class GetSampleNotesQueryHandler : IRequestHandler<GetSampleNotesQuery, IReadOnlyCollection<SampleNoteDto>>
{
    public Task<IReadOnlyCollection<SampleNoteDto>> Handle(GetSampleNotesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SampleNoteDto> notes = new[]
        {
            new SampleNoteDto(Guid.NewGuid(), "CQRS in practice", "Command handlers mutate state, queries read state.", DateTime.UtcNow),
            new SampleNoteDto(Guid.NewGuid(), "Validation pipeline", "Validators run automatically through pipeline behaviors.", DateTime.UtcNow),
        };

        return Task.FromResult(notes);
    }
}