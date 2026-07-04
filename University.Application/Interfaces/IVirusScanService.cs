namespace University.Application.Interfaces;

/// <summary>
/// Result returned by <see cref="IVirusScanService"/>.
/// </summary>
public enum VirusScanResult
{
    /// <summary>Scan completed; no threats detected.</summary>
    Clean,

    /// <summary>Scan completed; malicious content detected.</summary>
    Infected,

    /// <summary>No scanner is available; file was not scanned.</summary>
    Skipped,
}

/// <summary>
/// Abstracts virus/malware scanning for uploaded artifacts.
/// </summary>
public interface IVirusScanService
{
    /// <summary>
    /// Scans the artifact identified by <paramref name="storageKey"/> and returns the result.
    /// </summary>
    Task<VirusScanResult> ScanAsync(string storageKey, CancellationToken ct = default);
}
