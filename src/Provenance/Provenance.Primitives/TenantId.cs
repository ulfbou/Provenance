using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a canonical tenant identifier used for tenant isolation.
/// </summary>
/// <remarks>
/// <para>
/// The canonical form is trimmed, lowercase, 2 to 64 characters, and uses only lowercase letters,
/// digits, underscore, hyphen, and period.
/// </para>
/// <para>
/// Path traversal and path separator characters are rejected to preserve tenant isolation and path safety.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct TenantId
{
    private static readonly Regex ValidationRegex =
        new(@"^[a-z0-9_\-\.]{2,64}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the canonical tenant identifier.
    /// </summary>
    /// <value>
    /// A normalized, path-safe tenant identifier.
    /// </value>
    public string Value { get; }

    private TenantId(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a validated tenant identifier.
    /// </summary>
    /// <param name="value">
    /// The tenant identifier to validate. The value is trimmed and normalized to lowercase.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="TenantId"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.identity.blank_tenant</c></description></item>
    /// <item><description><c>provenance.identity.security_violation</c></description></item>
    /// </list>
    /// </returns>
    public static Result<TenantId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<TenantId>(ProvenanceErrors.Identity.BlankTenant);
        }

        string normalized = value.Trim().ToLowerInvariant();

        if (normalized.Contains("..") || normalized.Contains('/') || normalized.Contains('\\'))
        {
            return Result.Failure<TenantId>(ProvenanceErrors.Identity.SecurityViolation);
        }

        if (!ValidationRegex.IsMatch(normalized))
        {
            return Result.Failure<TenantId>(ProvenanceErrors.Identity.SecurityViolation);
        }

        return Result.Success(new TenantId(normalized));
    }

    /// <summary>
    /// Returns the canonical tenant identifier.
    /// </summary>
    /// <returns>
    /// The canonical tenant identifier string.
    /// </returns>
    public override string ToString() => Value;
}