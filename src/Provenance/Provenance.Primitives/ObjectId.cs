using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a validated object identifier within a stream.
/// </summary>
/// <remarks>
/// <para>
/// The value is trimmed but otherwise preserves casing and allowed path-like separators.
/// </para>
/// <para>
/// Allowed length is 1 to 256 characters. Allowed characters are letters, digits, underscore,
/// hyphen, period, slash, and colon.
/// </para>
/// <para>
/// Path traversal sequences (<c>..</c>) are rejected.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct ObjectId
{
    private static readonly Regex ValidRegex =
        new(@"^[a-zA-Z0-9_\-\.\/:]{1,256}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the validated object identifier.
    /// </summary>
    /// <value>
    /// A trimmed object identifier that satisfies the allowed character and path-safety rules.
    /// </value>
    public string Value { get; }

    private ObjectId(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a validated object identifier.
    /// </summary>
    /// <param name="value">
    /// The object identifier to validate. The value is trimmed before validation.
    /// </param>
    /// <returns>
    /// A successful result containing an <see cref="ObjectId"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.identity.blank_object_id</c></description></item>
    /// <item><description><c>provenance.identity.security_violation</c></description></item>
    /// </list>
    /// </returns>
    public static Result<ObjectId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<ObjectId>(ProvenanceErrors.Identity.BlankObjectId);
        }

        string normalized = value.Trim();

        if (normalized.Contains(".."))
        {
            return Result.Failure<ObjectId>(ProvenanceErrors.Identity.SecurityViolation);
        }

        if (!ValidRegex.IsMatch(normalized))
        {
            return Result.Failure<ObjectId>(ProvenanceErrors.Identity.SecurityViolation);
        }

        return Result.Success(new ObjectId(normalized));
    }

    /// <summary>
    /// Returns the validated object identifier.
    /// </summary>
    /// <returns>
    /// The validated object identifier string.
    /// </returns>
    public override string ToString() => Value;
}