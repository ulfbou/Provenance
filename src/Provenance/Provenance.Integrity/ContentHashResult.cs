using Dx.Domain;

using Provenance.Integrity;
using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>
/// Represents the canonical result of hashing content in an <see cref="IContentHasher"/>.
/// </summary>
/// <remarks>
/// <para>
/// The content identifier is produced from the bytes read from the source stream and is already canonical.
/// </para>
/// <para>
/// The byte count records the exact number of bytes hashed and is non-negative.
/// </para>
/// </remarks>
public class ContentHashResult
{
    /// <summary>
    /// Gets the computed content identifier.
    /// </summary>
    /// <value>
    /// The canonical <see cref="ContentId"/> computed by the content hasher.
    /// </value>
    public ContentId ContentId { get; init; }

    /// <summary>
    /// Gets the size of the content in bytes.
    /// </summary>
    /// <value>
    /// A non-negative byte count.
    /// </value>
    public long SizeBytes { get; init; }

    private ContentHashResult(ContentId contentId, long sizeBytes)
    {
        ContentId = contentId;
        SizeBytes = sizeBytes;
    }

    /// <summary>
    /// Creates a validated content hash result.
    /// </summary>
    /// <param name="contentId">
    /// The canonical content identifier computed by the content hasher.
    /// </param>
    /// <param name="sizeBytes">
    /// The number of bytes hashed. The value must not be negative.
    /// </param>
    /// <returns>
    /// A successful result containing a <see cref="ContentHashResult"/>, or a failure result with
    /// <c>provenance.integrity.negative_byte_count</c>.
    /// </returns>
    public static Result<ContentHashResult> Create(ContentId contentId, long sizeBytes)
    {
        if (contentId == default)
        {
            return Result.Failure<ContentHashResult>(ProvenanceErrors.Integrity.BlankContent);
        }

        if (sizeBytes < 0)
        {
            return Result.Failure<ContentHashResult>(ProvenanceErrors.Integrity.NegativeByteCount);
        }

        return Result.Success(new ContentHashResult(contentId, sizeBytes));
    }
}
