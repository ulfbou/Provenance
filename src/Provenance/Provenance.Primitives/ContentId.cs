using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a canonical content-addressed identifier for persisted Provenance content.
/// </summary>
/// <remarks>
/// <para>
/// v0.1.0-alpha supports SHA-256 only.
/// </para>
/// <para>
/// The canonical string form is <c>sha256:&lt;64 lowercase hex characters&gt;</c>.
/// </para>
/// <para>
/// Input is trimmed and normalized to lowercase before validation. The resulting value is immutable,
/// thread-safe, and uses value-based equality.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct ContentId
{
    private const string SupportedAlgorithm = "sha256";
    private const int Sha256HexLength = 64;

    private static readonly Regex HexRegex =
        new("^[a-f0-9]{64}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the canonical content identifier string.
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
    /// Gets the canonical SHA-256 hexadecimal digest.
    /// </summary>
    /// <value>
    /// A 64-character lowercase hexadecimal string.
    /// </value>
    public string Hex { get; }

    private ContentId(string algorithm, string hex)
    {
        Algorithm = algorithm;
        Hex = hex;
    }

    /// <summary>
    /// Creates a validated content identifier from an algorithm and hexadecimal digest.
    /// </summary>
    /// <param name="algorithm">
    /// The hash algorithm. The value is trimmed, normalized to lowercase, and must be <c>sha256</c>.
    /// </param>
    /// <param name="hex">
    /// The SHA-256 hexadecimal digest. The value is trimmed, normalized to lowercase, and must contain
    /// exactly 64 hexadecimal characters.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="ContentId"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integrity.blank_algorithm</c></description></item>
    /// <item><description><c>provenance.integrity.unsupported_algorithm</c></description></item>
    /// <item><description><c>provenance.integrity.blank_hex_value</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_length</c></description></item>
    /// <item><description><c>provenance.integrity.invalid_hex_format</c></description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The returned identifier is already canonical and safe for persistence, comparison, and content-addressed storage.
    /// </remarks>
    public static Result<ContentId> Create(string algorithm, string hex)
    {
        if (string.IsNullOrWhiteSpace(algorithm))
        {
            return Result.Failure<ContentId>(ProvenanceErrors.Integrity.BlankAlgorithm);
        }

        string normalizedAlgorithm = algorithm.Trim().ToLowerInvariant();

        if (normalizedAlgorithm != SupportedAlgorithm)
        {
            return Result.Failure<ContentId>(ProvenanceErrors.Integrity.UnsupportedAlgorithm);
        }

        if (string.IsNullOrWhiteSpace(hex))
        {
            return Result.Failure<ContentId>(ProvenanceErrors.Integrity.BlankHexValue);
        }

        string normalizedHex = hex.Trim().ToLowerInvariant();

        if (normalizedHex.Length != Sha256HexLength)
        {
            return Result.Failure<ContentId>(ProvenanceErrors.Integrity.InvalidHexLength);
        }

        if (!HexRegex.IsMatch(normalizedHex))
        {
            return Result.Failure<ContentId>(ProvenanceErrors.Integrity.InvalidHexFormat);
        }

        return Result.Success(new ContentId(normalizedAlgorithm, normalizedHex));
    }

    /// <summary>
    /// Parses a content identifier from its string representation.
    /// </summary>
    /// <param name="rawString">
    /// The raw content identifier. The value is trimmed and normalized to lowercase before validation. Accepted
    /// forms are <c>sha256:&lt;64 lowercase hex characters&gt;</c> and a bare SHA-256 hexadecimal digest.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="ContentId"/>, or a failure result with one of:
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
    /// The returned identifier is canonical and uses lowercase hexadecimal.
    /// </para>
    /// </remarks>
    public static Result<ContentId> Parse(string rawString)
    {
        if (string.IsNullOrWhiteSpace(rawString))
        {
            return Result.Failure<ContentId>(ProvenanceErrors.Integrity.BlankHexValue);
        }

        string clean = rawString.Trim().ToLowerInvariant();
        int separatorIndex = clean.IndexOf(':');

        if (separatorIndex == -1)
        {
            return Create(SupportedAlgorithm, clean);
        }

        string algorithm = clean[..separatorIndex];
        string hex = clean[(separatorIndex + 1)..];

        return Create(algorithm, hex);
    }

    /// <summary>
    /// Returns the canonical string representation of this content identifier.
    /// </summary>
    /// <returns>
    /// The canonical value in the form <c>sha256:&lt;64 lowercase hex characters&gt;</c>.
    /// </returns>
    public override string ToString() => Value;
}
