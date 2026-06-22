using Dx.Domain;

using Provenance.Primitives.Errors;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>
/// Produces deterministic unsigned manifest signatures for Provenance alpha builds.
/// </summary>
/// <remarks>
/// <para>
/// The generated token is derived from the manifest payload and is stable for identical payload bytes.
/// </para>
/// <para>
/// The returned trust level is fixed to <see cref="SignatureTrustLevel.UntrustedInternalMarker"/>.
/// </para>
/// </remarks>
public sealed class UnsignedAlphaManifestSigner : IRunManifestSigner
{
    /// <summary>
    /// Gets the fixed trust level assigned to unsigned alpha signatures.
    /// </summary>
    /// <value>
    /// Always <see cref="SignatureTrustLevel.UntrustedInternalMarker"/>.
    /// </value>
    public SignatureTrustLevel DefaultLevel => SignatureTrustLevel.UntrustedInternalMarker;

    /// <summary>
    /// Signs the supplied manifest payload deterministically.
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
    /// <remarks>
    /// Identical payload text yields identical tokens for the same implementation version.
    /// </remarks>
    public ValueTask<Result<ManifestSignature>> SignAsync(string manifestPayload, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(manifestPayload))
        {
            return ValueTask.FromResult(Result.Failure<ManifestSignature>(ProvenanceErrors.Manifest.BlankPayload));
        }

        if (ct.IsCancellationRequested)
        {
            return ValueTask.FromResult(Result.Failure<ManifestSignature>(ProvenanceErrors.Integrity.OperationCancelled));
        }

        using var sha = SHA256.Create();
        var hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(manifestPayload))).ToLowerInvariant();
        var token = $"unsigned-v0.1.0-alpha-{hash[..16]}";
        var result = ManifestSignature.Create(token, DefaultLevel);

        if (result.IsFailure)
        {
            return ValueTask.FromResult(Result.Failure<ManifestSignature>(result.Error));
        }

        return ValueTask.FromResult(result);
    }
}
