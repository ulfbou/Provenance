using Dx.Domain;

using Provenance.Primitives.Errors;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

public interface IRunManifestSigner
{
    SignatureTrustLevel DefaultLevel { get; }
    ValueTask<Result<ManifestSignature>> SignAsync(string manifestPayload, CancellationToken ct);
}

public sealed class UnsignedAlphaManifestSigner : IRunManifestSigner
{
    public SignatureTrustLevel DefaultLevel => SignatureTrustLevel.UntrustedInternalMarker;

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
