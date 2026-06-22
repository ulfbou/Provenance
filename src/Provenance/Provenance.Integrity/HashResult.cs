using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>
/// Represents the canonical result of hashing Provenance content.
/// </summary>
/// <remarks>
/// <para>
/// The algorithm is fixed to <c>sha256</c> in v0.1.0-alpha, and the hexadecimal value is canonical lowercase
/// hexadecimal.
/// </para>
/// <para>
/// The byte count records the exact number of bytes processed to produce the hash and is always non-negative.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct HashResult
{
    /// <summary>
    /// Gets the hashing algorithm used to produce the result.
    /// </summary>
    /// <value>
    /// The canonical hashing algorithm name <c>sha256</c>.
    /// </value>
    public string Algorithm { get; }

    /// <summary>
    /// Gets the canonical hexadecimal hash value.
    /// </summary>
    /// <value>
    /// A 64-character lowercase hexadecimal string for <c>sha256</c>.
    /// </value>
    public string HexValue { get; }

    /// <summary>
    /// Gets the number of bytes processed to produce the hash value.
    /// </summary>
    /// <value>
    /// A non-negative byte count.
    /// </value>
    public long BytesProcessed { get; }

    private HashResult(string alg, string hex, long bytes) => (Algorithm, HexValue, BytesProcessed) = (alg, hex, bytes);

    /// <summary>
    /// Creates a validated content-hash result.
    /// </summary>
    /// <param name="algorithm">
    /// The hashing algorithm name. The value is validated as <c>sha256</c>.
    /// </param>
    /// <param name="hex">
    /// The hexadecimal hash value. The value is validated as canonical lowercase SHA-256 hexadecimal.
    /// </param>
    /// <param name="bytes">
    /// The number of bytes processed to produce the hash. The value must not be negative.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="HashResult"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integrity.unsupported_algorithm</c></description></item>
    /// <item><description><c>provenance.integrity.blank_hex_value</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_length</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_format</c></description></item>
    /// <item><description><c>provenance.integrity.negative_byte_count</c></description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The returned value is canonical and safe for persistence and diagnostics.
    /// </remarks>
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
