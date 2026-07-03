namespace University.Infrastructure.Services;

using University.Application.Interfaces;

/// <summary>
/// Stores artifact binaries on the local file system under a configured base path.
/// Suitable for development, single-node deployments, and integration tests.
/// </summary>
public sealed class LocalFileAttachmentStorageService : IAttachmentStorageService
{
    private readonly string _basePath;

    /// <param name="basePath">
    /// Absolute path to the root directory where attachments are stored.
    /// The directory is created on first use if it does not exist.
    /// </param>
    public LocalFileAttachmentStorageService(string basePath)
    {
        _basePath = Path.GetFullPath(basePath);
        Directory.CreateDirectory(_basePath);
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        // Date-partitioned layout avoids excessively large flat directories
        var datePath = DateTime.UtcNow.ToString("yyyy/MM/dd");
        var storageKey = $"{datePath}/{Guid.NewGuid():N}-{Path.GetFileName(fileName)}";

        var fullPath = ToAbsolutePath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fileStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await stream.CopyToAsync(fileStream, ct);

        return storageKey;
    }

    /// <inheritdoc />
    public Task<(Stream Stream, string ContentType)> DownloadAsync(string storageKey, CancellationToken ct = default)
    {
        var fullPath = ToAbsolutePath(storageKey);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Attachment not found for storage key: {storageKey}");

        // Caller is responsible for disposing the returned stream
        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 81920, useAsync: true);
        return Task.FromResult((stream, "application/octet-stream"));
    }

    /// <inheritdoc />
    public Task DeleteAsync(string storageKey, CancellationToken ct = default)
    {
        var fullPath = ToAbsolutePath(storageKey);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private string ToAbsolutePath(string storageKey)
    {
        var fullPath = Path.GetFullPath(
            Path.Combine(_basePath, storageKey.Replace('/', Path.DirectorySeparatorChar)));

        // Defence-in-depth: reject any key that resolves outside the configured storage root
        if (!fullPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"Storage key resolves outside the storage root. Key: '{storageKey}'");

        return fullPath;
    }
}
