using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>
/// Represents the canonical signature metadata for a Provenance run manifest.
/// </summary>
/// <remarks>
/// <para>
/// The token is opaque signature metadata and is preserved exactly as supplied after validation.
/// </para>
/// <para>
/// The trust level records the provenance of the signature token and is part of the public contract.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct ManifestSignature
{
    /// <summary>
    /// Gets the signature token.
    /// </summary>
    /// <value>
    /// The validated signature token string.
    /// </value>
    public string Token { get; }

    /// <summary>
    /// Gets the trust level assigned to the signature token.
    /// </summary>
    /// <value>
    /// The trust level associated with the token.
    /// </value>
    public SignatureTrustLevel TrustLevel { get; }

    private ManifestSignature(string token, SignatureTrustLevel level) => (Token, TrustLevel) = (token, level);

    /// <summary>
    /// Creates a validated manifest signature.
    /// </summary>
    /// <param name="token">
    /// The signature token. The value must not be blank.
    /// </param>
    /// <param name="level">
    /// The trust level assigned to the token.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="ManifestSignature"/>, or a failure result with
    /// <c>provenance.manifest.blank_token</c>.
    /// </returns>
    public static Result<ManifestSignature> Create(string token, SignatureTrustLevel level)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Failure<ManifestSignature>(ProvenanceErrors.Manifest.BlankToken);
        }

        return Result.Success(new ManifestSignature(token, level));
    }
}
