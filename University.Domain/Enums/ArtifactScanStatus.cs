namespace University.Domain.Enums;

/// <summary>
/// Represents the outcome of the virus-scan pipeline for a thesis artifact.
/// </summary>
public enum ArtifactScanStatus
{
    /// <summary>Upload accepted; scan not yet completed.</summary>
    Pending = 0,

    /// <summary>Scan completed; no threats detected.</summary>
    Clean = 1,

    /// <summary>Scan completed; malicious content detected.</summary>
    Infected = 2,

    /// <summary>No scan provider is configured; file accepted without scanning.</summary>
    Skipped = 3,
}
