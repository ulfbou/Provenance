using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a canonical source system identifier.
/// </summary>
/// <remarks>
/// <para>
/// Examples include <c>github.com</c> and <c>gitlab.internal</c>.
/// </para>
/// <para>
/// The canonical form is a trimmed, lowercase source-system key, 2 to 128 characters, and uses only lowercase
/// letters, digits, underscore, hyphen, and period.
/// </para>
/// <para>
/// Source-system identifiers commonly contain host-like values, but they are not required to be valid DNS hostnames.
/// </para>
/// <para>
/// Path traversal and path separator characters are rejected.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct SourceSystemId
{
    private static readonly Regex SystemRegex =
        new(@"^[a-z0-9_\-\.]{2,128}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the canonical source system identifier.
    /// </summary>
    /// <value>
    /// A normalized source-system key, commonly a host-like value such as <c>github.com</c>.
    /// </value>
    public string Value { get; }

    private SourceSystemId(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a validated source system identifier.
    /// </summary>
    /// <param name="value">
    /// The source system identifier to validate. The value is trimmed and normalized to lowercase.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="SourceSystemId"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.identity.blank_source_system</c></description></item>
    /// <item><description><c>provenance.identity.invalid_source_system</c></description></item>
    /// </list>
    /// </returns>
    public static Result<SourceSystemId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<SourceSystemId>(ProvenanceErrors.Identity.BlankSourceSystem);
        }

        string normalized = value.Trim().ToLowerInvariant();

        if (normalized.Contains("..") || normalized.Contains('/') || normalized.Contains('\\'))
        {
            return Result.Failure<SourceSystemId>(ProvenanceErrors.Identity.InvalidSourceSystem);
        }

        if (!SystemRegex.IsMatch(normalized))
        {
            return Result.Failure<SourceSystemId>(ProvenanceErrors.Identity.InvalidSourceSystem);
        }

        return Result.Success(new SourceSystemId(normalized));
    }

    /// <summary>
    /// Returns the canonical source system identifier.
    /// </summary>
    /// <returns>
    /// The canonical source system identifier string.
    /// </returns>
    public override string ToString() => Value;
}