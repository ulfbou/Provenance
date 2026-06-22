using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>Result of a content hash operation. Immutable and validated.</summary>
[ValueObject]
public readonly record struct HashResult
{
    /// <summary>
    /// The name of the hashing algorithm used, e.g. "sha256". This is not a free-form string; it must be one of the supported algorithms recognized by the system.
    /// </summary>
    /// <remarks>
    /// In v0.1.0-alpha, only "sha256" is supported. Future versions may support additional algorithms, but any algorithm used must be explicitly recognized and validated by the system.
    /// </remarks>
    public string Algorithm { get; }

    /// <summary>
    /// The hexadecimal string representation of the hash value. The format and length of this string depend on the hashing algorithm used. For example, a SHA-256 hash is typically represented as a 64-character hexadecimal string.
    /// </summary>
    public string HexValue { get; }

    /// <summary>The number of bytes processed to produce the hash value.</summary>
    public long BytesProcessed { get; }

    private HashResult(string alg, string hex, long bytes) => (Algorithm, HexValue, BytesProcessed) = (alg, hex, bytes);

    public static Result<HashResult> Create(string algorithm, string hex, long bytes)
    {
        if (algorithm != "sha256")
        {
            return Result.Failure<HashResult>(ProvenanceErrors.Integrity.UnsupportedAlgorithm);
        }

        // reuse ContentId validation
        var cid = ContentId.Create(algorithm, hex);
        if (cid.IsFailure) return Result.Failure<HashResult>(cid.Error);
        if (bytes < 0) return Result.Failure<HashResult>(ProvenanceErrors.Integrity.NegativeByteCount);

        return Result.Success(new HashResult(algorithm, hex, bytes));
    }
}
