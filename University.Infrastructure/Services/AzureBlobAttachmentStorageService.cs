namespace University.Infrastructure.Services;

using University.Application.Interfaces;

/// <summary>
/// Stub implementation for Azure Blob Storage.
/// Register by setting <c>Attachments:StorageProvider = "AzureBlob"</c> in configuration,
/// then replace this stub with a real implementation using the Azure.Storage.Blobs SDK.
/// </summary>
public sealed class AzureBlobAttachmentStorageService : IAttachmentStorageService
{
    private const string StubMessage =
        "Azure Blob storage provider is not yet configured. " +
        "Set 'Attachments:StorageProvider' to 'Local' or provide a real Azure Blob implementation.";

    public Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default) =>
        throw new NotImplementedException(StubMessage);

    public Task<(Stream Stream, string ContentType)> DownloadAsync(string storageKey, CancellationToken ct = default) =>
        throw new NotImplementedException(StubMessage);

    public Task DeleteAsync(string storageKey, CancellationToken ct = default) =>
        throw new NotImplementedException(StubMessage);
}
