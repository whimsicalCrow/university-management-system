namespace University.Application.Commands;

using MediatR;

/// <summary>
/// Uploads a thesis artifact through the storage pipeline:
/// validates extension/size, stores the binary, persists metadata,
/// and triggers the virus-scan hook.
/// </summary>
public class UploadAttachmentCommand : IRequest<UploadAttachmentResult>
{
    /// <summary>Email or username of the authenticated student (used to resolve the student profile).</summary>
    public required string IdentityUserName { get; set; }

    /// <summary>Thesis / topic identifier to associate the artifact with.</summary>
    public required Guid ThesisId { get; set; }

    /// <summary>Original client-supplied file name (will be sanitised server-side).</summary>
    public required string FileName { get; set; }

    /// <summary>MIME content-type reported by the client (magic-byte validation is also performed).</summary>
    public required string ContentType { get; set; }

    /// <summary>Readable stream containing the file bytes. Must be open and positioned at zero.</summary>
    public required Stream FileStream { get; set; }
}

/// <summary>
/// Result of <see cref="UploadAttachmentCommand"/>.
/// </summary>
public class UploadAttachmentResult
{
    public bool Success { get; private set; }
    public string? Error { get; private set; }
    public Guid ArtifactId { get; private set; }

    public static UploadAttachmentResult Ok(Guid artifactId) =>
        new() { Success = true, ArtifactId = artifactId };

    public static UploadAttachmentResult Fail(string error) =>
        new() { Success = false, Error = error };
}
