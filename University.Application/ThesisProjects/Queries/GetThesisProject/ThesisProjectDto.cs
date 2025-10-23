namespace University.Application.ThesisProjects.Queries.GetThesisProject;

public sealed record ThesisProjectDto(
    Guid Id,
    Guid StudentId,
    Guid SupervisorId,
    string Title,
    string Summary,
    string Status,
    IReadOnlyCollection<ThesisUpdateDto> Updates);

public sealed record ThesisUpdateDto(
    Guid Id,
    Guid AuthorId,
    string Note,
    DateTime OccurredOn,
    string Status,
    DateTime LastModifiedOn,
    IReadOnlyCollection<ThesisAttachmentDto> Attachments);

public sealed record ThesisAttachmentDto(
    string FileName,
    string ContentType,
    string BlobName,
    long SizeBytes);
