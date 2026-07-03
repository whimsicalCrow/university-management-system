namespace University.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using University.Application.Interfaces;

/// <summary>
/// Default virus scan implementation that skips scanning and logs the outcome.
/// Replace with a real antivirus integration (e.g., ClamAV, Windows Defender ATP)
/// by implementing <see cref="IVirusScanService"/> and registering it in DI.
/// </summary>
public sealed class NoOpVirusScanService : IVirusScanService
{
    private readonly ILogger<NoOpVirusScanService> _logger;

    public NoOpVirusScanService(ILogger<NoOpVirusScanService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<VirusScanResult> ScanAsync(string storageKey, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Virus scan skipped (no provider configured) for storage key: {StorageKey}", storageKey);
        return Task.FromResult(VirusScanResult.Skipped);
    }
}
