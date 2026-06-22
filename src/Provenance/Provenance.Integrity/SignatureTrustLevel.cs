namespace Provenance.Integrity;

/// <summary>
/// Identifies the trust level assigned to a manifest signature.
/// </summary>
/// <remarks>
/// <para>
/// The values are ordered from no trust to Cryptographically verified trust.
/// </para>
/// <para>
/// Callers must treat the value as contract metadata and not as a free-form severity level.
/// </para>
/// </remarks>
public enum SignatureTrustLevel
{
    /// <summary>
    /// No trust has been assigned to the signature.
    /// </summary>
    None = 0,

    /// <summary>
    /// The signature is an internal marker and is not Cryptographically verified.
    /// </summary>
    UntrustedInternalMarker = 1,

    /// <summary>
    /// The signature has been Cryptographically verified.
    /// </summary>
    CryptographicallyVerified = 2
}
