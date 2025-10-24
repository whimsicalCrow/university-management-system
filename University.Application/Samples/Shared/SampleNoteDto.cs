namespace University.Application.Samples.Shared;

public sealed record SampleNoteDto(Guid Id, string Title, string Summary, DateTime CreatedOnUtc);