using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a canonical stream identifier.
/// </summary>
/// <remarks>
/// <para>
/// The canonical form is trimmed, lowercase, 2 to 64 characters, and uses only lowercase letters,
/// digits, and underscore.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct StreamId
{
    private static readonly Regex ValidRegex =
        new(@"^[a-z0-9_]{2,64}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the canonical stream identifier.
    /// </summary>
    /// <value>
    /// A normalized stream identifier containing only lowercase letters, digits, and underscore.
    /// </value>
    public string Value { get; }

    private StreamId(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a validated stream identifier.
    /// </summary>
    /// <param name="value">
    /// The stream identifier to validate. The value is trimmed and normalized to lowercase.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="StreamId"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.identity.blank_stream</c></description></item>
    /// <item><description><c>provenance.identity.security_violation</c></description></item>
    /// </list>
    /// </returns>
    public static Result<StreamId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<StreamId>(ProvenanceErrors.Identity.BlankStream);
        }

        string normalized = value.Trim().ToLowerInvariant();

        if (!ValidRegex.IsMatch(normalized))
        {
            return Result.Failure<StreamId>(ProvenanceErrors.Identity.SecurityViolation);
        }

        return Result.Success(new StreamId(normalized));
    }

    /// <summary>
    /// Returns the canonical stream identifier.
    /// </summary>
    /// <returns>
    /// The canonical stream identifier string.
    /// </returns>
    public override string ToString() => Value;
}