using Dx.Domain;
using Dx.Domain.Annotations;

using Provenance.Primitives.Errors;

using System.IO;
using System.Text.RegularExpressions;

using static Dx.Domain.Dx;

namespace Provenance.Primitives;

/// <summary>
/// Represents a canonical repository identity.
/// </summary>
/// <remarks>
/// <para>
/// A repository key consists of host, owner, and repository name segments.
/// </para>
/// <para>
/// Each segment is trimmed, normalized to lowercase, and must contain 1 to 128 characters.
/// Allowed characters are lowercase letters, digits, underscore, hyphen, and period.
/// </para>
/// <para>
/// The host segment is a repository authority key. It commonly contains host-like values such as
/// <c>github.com</c>, but it is validated as a path-safe Provenance key segment rather than as a strict DNS hostname.
/// </para>
/// <para>
/// Path traversal and path separator characters are rejected.
/// </para>
/// </remarks>
[ValueObject]
public readonly record struct RepoKey
{
    private static readonly Regex SegmentRegex =
        new(@"^[a-z0-9_\-\.]{1,128}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the canonical repository host key segment.
    /// </summary>
    /// <value>
    /// A lowercase repository authority key, commonly a host-like value such as <c>github.com</c>.
    /// </value>
    public string Host { get; }

    /// <summary>
    /// Gets the canonical repository owner segment.
    /// </summary>
    /// <value>
    /// A lowercase repository owner, organization, or namespace.
    /// </value>
    public string Owner { get; }

    /// <summary>
    /// Gets the canonical repository name segment.
    /// </summary>
    /// <value>
    /// A lowercase repository name.
    /// </value>
    public string Name { get; }

    /// <summary>
    /// Gets the default relative local path derived from the canonical key.
    /// </summary>
    /// <remarks>
    /// The value is <c>Host/Owner/Name</c> using the OS directory separator.
    /// Segments are already validated to be path-safe.
    /// </remarks>
    public string LocalPath => Path.Combine(Host, Owner, Name);

    private RepoKey(string host, string owner, string name)
    {
        Host = host;
        Owner = owner;
        Name = name;
    }

    /// <summary>
    /// Creates a validated repository key.
    /// </summary>
    /// <param name="host">
    /// The repository host key segment. The value is trimmed and normalized to lowercase.
    /// </param>
    /// <param name="owner">
    /// The repository owner segment. The value is trimmed and normalized to lowercase.
    /// </param>
    /// <param name="name">
    /// The repository name segment. The value is trimmed and normalized to lowercase.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="RepoKey"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.identity.blank_host</c></description></item>
    /// <item><description><c>provenance.identity.blank_owner</c></description></item>
    /// <item><description><c>provenance.identity.blank_name</c></description></item>
    /// <item><description><c>provenance.identity.security_violation</c></description></item>
    /// </list>
    /// </returns>
    public static Result<RepoKey> Create(string host, string owner, string name)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return Result.Failure<RepoKey>(ProvenanceErrors.Identity.BlankHost);
        }

        if (string.IsNullOrWhiteSpace(owner))
        {
            return Result.Failure<RepoKey>(ProvenanceErrors.Identity.BlankOwner);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<RepoKey>(ProvenanceErrors.Identity.BlankName);
        }

        string normalizedHost = host.Trim().ToLowerInvariant();
        string normalizedOwner = owner.Trim().ToLowerInvariant();
        string normalizedName = name.Trim().ToLowerInvariant();

        if (ContainsUnsafePathSegment(normalizedHost) ||
            ContainsUnsafePathSegment(normalizedOwner) ||
            ContainsUnsafePathSegment(normalizedName))
        {
            return Result.Failure<RepoKey>(ProvenanceErrors.Identity.SecurityViolation);
        }

        if (!SegmentRegex.IsMatch(normalizedHost) ||
            !SegmentRegex.IsMatch(normalizedOwner) ||
            !SegmentRegex.IsMatch(normalizedName))
        {
            return Result.Failure<RepoKey>(ProvenanceErrors.Identity.SecurityViolation);
        }

        return Result.Success(new RepoKey(normalizedHost, normalizedOwner, normalizedName));
    }

    /// <summary>
    /// Returns the canonical repository key string.
    /// </summary>
    /// <returns>
    /// The canonical repository key in the form <c>host/owner/name</c>.
    /// </returns>
    public override string ToString() => $"{Host}/{Owner}/{Name}";

    private static bool ContainsUnsafePathSegment(string value) =>
        value.Contains("..") || value.Contains('/') || value.Contains('\\');
}