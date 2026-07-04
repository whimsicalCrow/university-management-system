namespace University.Application.Commands;

using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using University.Application.Interfaces;
using University.Domain.Entities;
using University.Domain.Enums;

/// <summary>
/// Handles <see cref="UploadAttachmentCommand"/>:
/// validates the upload, delegates binary storage to <see cref="IAttachmentStorageService"/>,
/// persists metadata via <see cref="IThesisArtifactRepository"/>, and triggers virus scanning.
/// </summary>
public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, UploadAttachmentResult>
{
    private static readonly string[] DefaultAllowedExtensions = [".pdf", ".docx", ".pptx", ".zip"];
    private const long DefaultMaxFileSizeBytes = 20_971_520L; // 20 MB

    private readonly UserManager<IdentityUser> _userManager;
    private readonly IThesisArtifactRepository _repository;
    private readonly IAttachmentStorageService _storage;
    private readonly IVirusScanService _virusScan;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UploadAttachmentCommandHandler> _logger;

    public UploadAttachmentCommandHandler(
        UserManager<IdentityUser> userManager,
        IThesisArtifactRepository repository,
        IAttachmentStorageService storage,
        IVirusScanService virusScan,
        IConfiguration configuration,
        ILogger<UploadAttachmentCommandHandler> logger)
    {
        _userManager = userManager;
        _repository = repository;
        _storage = storage;
        _virusScan = virusScan;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<UploadAttachmentResult> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        var allowedExtensions = _configuration
            .GetSection("Attachments:AllowedExtensions")
            .Get<string[]>() ?? DefaultAllowedExtensions;

        var maxFileSizeBytes = _configuration
            .GetValue<long>("Attachments:MaxFileSizeBytes", DefaultMaxFileSizeBytes);

        // Validate extension
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            return UploadAttachmentResult.Fail(
                $"File type '{extension}' is not allowed. Accepted: {string.Join(", ", allowedExtensions)}");
        }

        // Buffer the stream so we can inspect magic bytes and validate size
        using var buffer = new MemoryStream();
        await request.FileStream.CopyToAsync(buffer, cancellationToken);
        var data = buffer.ToArray();

        if (data.Length == 0)
            return UploadAttachmentResult.Fail("Uploaded file is empty.");

        if (data.LongLength > maxFileSizeBytes)
        {
            return UploadAttachmentResult.Fail(
                $"File exceeds the maximum allowed size of {maxFileSizeBytes / 1_048_576} MB.");
        }

        // Magic-byte validation (server-side MIME check)
        if (!ValidateMagicBytes(data, extension))
        {
            return UploadAttachmentResult.Fail(
                $"File content does not match the declared extension '{extension}'.");
        }

        // Sanitise the client-supplied file name
        var sanitisedFileName = SanitiseFileName(request.FileName);

        // Resolve the student profile
        var identityUser = await _userManager.FindByEmailAsync(request.IdentityUserName)
                           ?? await _userManager.FindByNameAsync(request.IdentityUserName);

        if (identityUser is null)
        {
            _logger.LogWarning("Attachment upload rejected: identity user not found ({UserName})", request.IdentityUserName);
            return UploadAttachmentResult.Fail("Authenticated user session is invalid.");
        }

        var studentId = await _repository.FindStudentIdByIdentityUserIdAsync(identityUser.Id, cancellationToken);
        if (!studentId.HasValue)
        {
            _logger.LogWarning("Attachment upload rejected: student profile not found for user {UserId}", identityUser.Id);
            return UploadAttachmentResult.Fail("Only student users can upload thesis artifacts.");
        }

        // Upload binary to storage provider
        string storageKey;
        try
        {
            using var uploadStream = new MemoryStream(data);
            storageKey = await _storage.UploadAsync(
                uploadStream,
                sanitisedFileName,
                string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Storage upload failed for {FileName}", sanitisedFileName);
            return UploadAttachmentResult.Fail("Storage upload failed. Please try again.");
        }

        // Persist metadata
        var artifact = ThesisArtifact.CreateWithStorageKey(
            request.ThesisId,
            studentId.Value,
            identityUser.Id,
            sanitisedFileName,
            string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType,
            data.LongLength,
            storageKey);

        var artifactId = await _repository.SaveAsync(artifact, cancellationToken);

        // Virus scan (non-blocking failure)
        try
        {
            var scanResult = await _virusScan.ScanAsync(storageKey, cancellationToken);
            var scanStatus = scanResult switch
            {
                VirusScanResult.Clean    => ArtifactScanStatus.Clean,
                VirusScanResult.Infected => ArtifactScanStatus.Infected,
                _                        => ArtifactScanStatus.Skipped,
            };

            await _repository.UpdateScanStatusAsync(artifactId, scanStatus, cancellationToken);

            if (scanStatus == ArtifactScanStatus.Infected)
            {
                _logger.LogWarning("Artifact {ArtifactId} flagged by virus scanner; purging from storage", artifactId);
                try { await _storage.DeleteAsync(storageKey, cancellationToken); }
                catch (Exception purgeEx)
                {
                    _logger.LogError(purgeEx, "Failed to purge infected artifact {ArtifactId} from storage", artifactId);
                }

                return UploadAttachmentResult.Fail(
                    "The uploaded file was flagged by the security scanner and cannot be stored.");
            }

            _logger.LogInformation(
                "Artifact {ArtifactId} uploaded and scanned (status: {ScanStatus})", artifactId, scanStatus);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex, "Virus scan failed for artifact {ArtifactId}; continuing with Skipped status", artifactId);
            await _repository.UpdateScanStatusAsync(artifactId, ArtifactScanStatus.Skipped, cancellationToken);
        }

        return UploadAttachmentResult.Ok(artifactId);
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static bool ValidateMagicBytes(byte[] data, string extension) =>
        extension switch
        {
            ".pdf" =>
                data.Length >= 4
                && data[0] == 0x25 && data[1] == 0x50 && data[2] == 0x44 && data[3] == 0x46, // %PDF
            ".zip" or ".docx" or ".pptx" =>
                data.Length >= 4
                && data[0] == 0x50 && data[1] == 0x4B && data[2] == 0x03 && data[3] == 0x04, // PK\x03\x04
            _ => false,
        };

    private static string SanitiseFileName(string fileName)
    {
        // Strip any directory component, then replace invalid characters
        var name = Path.GetFileName(fileName);
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        if (name.Length <= 255)
            return name;

        // Preserve extension when truncating so the file remains recognisable
        var ext = Path.GetExtension(name); // e.g. ".pdf"
        var maxBase = 255 - ext.Length;
        return name[..maxBase] + ext;
    }
}
