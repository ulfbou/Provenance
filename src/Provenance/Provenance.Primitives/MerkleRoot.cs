using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a canonical Merkle tree root identifier.
/// </summary>
/// <remarks>
/// <para>
/// v0.1.0-alpha supports SHA-256 Merkle roots only.
/// </para>
/// <para>
/// The canonical string form is <c>sha256:&lt;64 lowercase hex characters&gt;</c>.
/// </para>
/// <para>
/// Input is trimmed and normalized to lowercase before validation. The resulting value is immutable
/// and safe for persistence.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct MerkleRoot
{
    private const string SupportedAlgorithm = "sha256";
    private const int Sha256HexLength = 64;

    private static readonly Regex HexRegex =
        new("^[a-f0-9]{64}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the canonical Merkle root identifier.
    /// </summary>
    /// <value>
    /// A value in the form <c>sha256:&lt;64 lowercase hex characters&gt;</c>.
    /// </value>
    public string Value => $"{Algorithm}:{Hex}";

    /// <summary>
    /// Gets the canonical hash algorithm.
    /// </summary>
    /// <value>
    /// Always <c>sha256</c> in v0.1.0-alpha.
    /// </value>
    public string Algorithm { get; }

    /// <summary>
    /// Gets the canonical SHA-256 hexadecimal digest for the Merkle root.
    /// </summary>
    /// <value>
    /// A 64-character lowercase hexadecimal string.
    /// </value>
    public string Hex { get; }

    private MerkleRoot(string algorithm, string hex)
    {
        Algorithm = algorithm;
        Hex = hex;
    }

    /// <summary>
    /// Creates a validated Merkle root from a SHA-256 hexadecimal digest.
    /// </summary>
    /// <param name="hashHex">
    /// The SHA-256 hexadecimal digest. The value is trimmed, normalized to lowercase, and must contain
    /// exactly 64 hexadecimal characters.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="MerkleRoot"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integrity.blank_hex_value</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_length</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_format</c></description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// This factory accepts the bare SHA-256 hexadecimal digest. Use <see cref="Parse(string)"/> when input is either
    /// bare hex or the canonical prefixed string form.
    /// </remarks>
    public static Result<MerkleRoot> Create(string hashHex)
    {
        if (string.IsNullOrWhiteSpace(hashHex))
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.BlankHexValue);
        }

        string normalizedHex = hashHex.Trim().ToLowerInvariant();

        if (normalizedHex.Length != Sha256HexLength)
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.InvalidHexLength);
        }

        if (!HexRegex.IsMatch(normalizedHex))
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.InvalidHexFormat);
        }

        return Result.Success(new MerkleRoot(SupportedAlgorithm, normalizedHex));
    }

    /// <summary>
    /// Parses a Merkle root from its string representation.
    /// </summary>
    /// <param name="rawString">
    /// The raw Merkle root. The value is trimmed and normalized to lowercase before validation. Accepted forms are
    /// <c>sha256:&lt;64 lowercase hex characters&gt;</c> and a bare SHA-256 hexadecimal digest.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="MerkleRoot"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integrity.blank_algorithm</c></description></item>
    /// <item><description><c>provenance.integrity.unsupported_algorithm</c></description></item>
    /// <item><description><c>provenance.integrity.blank_hex_value</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_length</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_format</c></description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// If no algorithm prefix is present, <c>sha256</c> is assumed.
    /// </para>
    /// <para>
    /// The returned root is canonical and uses lowercase hexadecimal.
    /// </para>
    /// </remarks>
    public static Result<MerkleRoot> Parse(string rawString)
    {
        if (string.IsNullOrWhiteSpace(rawString))
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.BlankHexValue);
        }

        string clean = rawString.Trim().ToLowerInvariant();
        int separatorIndex = clean.IndexOf(':');

        if (separatorIndex == -1)
        {
            return Create(clean);
        }

        string algorithm = clean[..separatorIndex];
        string hex = clean[(separatorIndex + 1)..];

        if (string.IsNullOrWhiteSpace(algorithm))
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.BlankAlgorithm);
        }

        if (algorithm != SupportedAlgorithm)
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.UnsupportedAlgorithm);
        }

        return Create(hex);
    }

    /// <summary>
    /// Returns the canonical string representation of this Merkle root.
    /// </summary>
    /// <returns>
    /// The canonical value in the form <c>sha256:&lt;64 lowercase hex characters&gt;</c>.
    /// </returns>
    public override string ToString() => Value;
}
