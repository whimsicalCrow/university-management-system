namespace University.Application.Interfaces;

/// <summary>
/// Abstracts binary storage for thesis artifacts.
/// Implementations may target local disk, Azure Blob, S3, etc.
/// </summary>
public interface IAttachmentStorageService
{
    /// <summary>
    /// Stores the stream content and returns an opaque storage key.
    /// The key can later be passed to <see cref="DownloadAsync"/> or <see cref="DeleteAsync"/>.
    /// </summary>
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Retrieves the binary content identified by <paramref name="storageKey"/>.
    /// The returned <see cref="Stream"/> must be disposed by the caller.
    /// </summary>
    Task<(Stream Stream, string ContentType)> DownloadAsync(string storageKey, CancellationToken ct = default);

    /// <summary>
    /// Permanently removes the binary identified by <paramref name="storageKey"/>.
    /// Implementations must be idempotent (no-op if the key does not exist).
    /// </summary>
    Task DeleteAsync(string storageKey, CancellationToken ct = default);
}
