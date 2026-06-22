using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

[ValueObject]
public readonly record struct ManifestSignature
{
    public string Token { get; }
    public SignatureTrustLevel TrustLevel { get; }

    private ManifestSignature(string token, SignatureTrustLevel level) => (Token, TrustLevel) = (token, level);

    public static Result<ManifestSignature> Create(string token, SignatureTrustLevel level)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Failure<ManifestSignature>(ProvenanceErrors.Manifest.BlankToken); 
        }

        return Result.Success(new ManifestSignature(token, level));
    }
}
