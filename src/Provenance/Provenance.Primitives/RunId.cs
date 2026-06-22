using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a canonical pipeline run identifier.
/// </summary>
/// <remarks>
/// <para>
/// The canonical form is trimmed, lowercase, path-safe, 1 to 128 characters, and uses only lowercase letters,
/// digits, underscore, hyphen, and period.
/// </para>
/// <para>
/// Path traversal and path separator characters are rejected.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct RunId
{
    private static readonly Regex ValidRegex =
        new(@"^[a-z0-9_\-\.]{1,128}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the canonical run identifier.
    /// </summary>
    /// <value>
    /// A trimmed, lowercase, path-safe run identifier.
    /// </value>
    public string Value { get; }

    private RunId(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a validated run identifier.
    /// </summary>
    /// <param name="value">
    /// The run identifier to validate. The value is trimmed and normalized to lowercase.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="RunId"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.identity.blank_run_id</c></description></item>
    /// <item><description><c>provenance.identity.security_violation</c></description></item>
    /// </list>
    /// </returns>
    public static Result<RunId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<RunId>(ProvenanceErrors.Identity.BlankRunId);
        }

        string normalized = value.Trim().ToLowerInvariant();

        if (normalized.Contains('/') || normalized.Contains('\\') || normalized.Contains(".."))
        {
            return Result.Failure<RunId>(ProvenanceErrors.Identity.SecurityViolation);
        }

        if (!ValidRegex.IsMatch(normalized))
        {
            return Result.Failure<RunId>(ProvenanceErrors.Identity.SecurityViolation);
        }

        return Result.Success(new RunId(normalized));
    }

    /// <summary>
    /// Returns the canonical run identifier.
    /// </summary>
    /// <returns>
    /// The canonical run identifier string.
    /// </returns>
    public override string ToString() => Value;
}