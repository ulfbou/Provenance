using Dx.Domain;

using System.Threading;
using System.Threading.Tasks;

namespace Provenance.Integrity;

/// <summary>
/// Defines deterministic manifest signing for Provenance run manifests.
/// </summary>
/// <remarks>
/// <para>
/// Implementations sign the manifest payload without exposing raw secrets or exception details.
/// </para>
/// <para>
/// The signer contract is Result-first. Validation and operational failures must be surfaced through stable
/// Provenance error codes.
/// </para>
/// </remarks>
public interface IRunManifestSigner
{
    /// <summary>
    /// Gets the default trust level assigned to signatures produced by the signer.
    /// </summary>
    /// <value>
    /// The trust level that callers should associate with signatures produced by this signer.
    /// </value>
    SignatureTrustLevel DefaultLevel { get; }

    /// <summary>
    /// Signs the supplied manifest payload.
    /// </summary>
    /// <param name="manifestPayload">
    /// The manifest payload to sign. The value must not be blank.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the signing operation.
    /// </param>
    /// <returns>
    /// A successful result containing a <see cref="ManifestSignature"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.manifest.blank_payload</c></description></item>
    /// <item><description><c>provenance.integrity.operation_cancelled</c></description></item>
    /// </list>
    /// </returns>
    ValueTask<Result<ManifestSignature>> SignAsync(string manifestPayload, CancellationToken ct);
}
