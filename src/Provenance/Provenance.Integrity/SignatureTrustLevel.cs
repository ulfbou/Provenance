namespace Provenance.Integrity;

/// <summary>
/// Represents the trust level of a manifest signature. This is an immutable value object that indicates the
/// degree of trust that can be placed in a given manifest signature, based on how it was generated and verified.
/// </summary>
/// <remarks>
/// The trust levels are defined as follows:
/// - None: No trust can be placed in the signature. This may indicate that the signature is missing, invalid, or 
/// unrecognized.
/// - UntrustedInternalMarker: The signature is an internal marker that indicates the manifest has been processed 
///  by the system, but it has not been cryptographically verified. This may be used for manifests that are 
///  generated or modified internally, but have not undergone a full signing process.
/// - CryptographicallyVerified: The signature has been cryptographically verified and can be trusted to 
/// accurately represent the integrity of the manifest. This indicates that the manifest has been signed using 
/// a recognized signing process and the signature has been successfully validated.
/// </remarks>
public enum SignatureTrustLevel
{
    /// <summary>
    /// No trust can be placed in the signature. This may indicate that the signature is missing, invalid, or 
    /// unrecognized.
    /// </summary>
    None = 0,

    /// <summary>
    /// The signature is an internal marker that indicates the manifest has been processed by the system, but it 
    /// has not been cryptographically verified. This may be used for manifests that are generated or modified 
    /// internally, but have not undergone a full signing process.
    /// </summary>
    UntrustedInternalMarker = 1,

    /// <summary>
    /// The signature has been cryptographically verified and can be trusted to accurately represent the 
    /// integrity of the manifest. This indicates that the manifest has been signed using a recognized signing 
    /// process and the signature has been successfully validated.
    /// </summary>
    CryptographicallyVerified = 2
}
