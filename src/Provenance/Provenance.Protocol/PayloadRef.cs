using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Protocol;

/// <summary>
/// Represents a reference to protocol payload content.
/// </summary>
/// <remarks>
/// <para>
/// The payload reference binds protocol metadata to a content-addressed identifier from <c>Provenance.Primitives</c>.
/// </para>
/// <para>
/// The payload size is measured in bytes and must not be negative.
/// </para>
/// </remarks>
public sealed record PayloadRef
{
    /// <summary>
    /// Gets the content-addressed identifier for the payload.
    /// </summary>
    /// <value>
    /// A validated <see cref="ContentId"/> for the persisted payload content.
    /// </value>
    public ContentId Cid { get; init; }

    /// <summary>
    /// Gets the payload size in bytes.
    /// </summary>
    /// <value>
    /// A non-negative byte count.
    /// </value>
    public long SizeBytes { get; init; }

    private PayloadRef(ContentId cid, long sizeBytes)
    {
        Cid = cid;
        SizeBytes = sizeBytes;
    }

    /// <summary>
    /// Creates a validated payload reference.
    /// </summary>
    /// <param name="cid">
    /// The content identifier for the payload.
    /// </param>
    /// <param name="sizeBytes">
    /// The payload size in bytes. Must not be negative.
    /// </param>
    /// <returns>
    /// A successful result containing a <see cref="PayloadRef"/>, or a failure result with
    /// <c>provenance.protocol.payload_size_negative</c>.
    /// </returns>
    public static Result<PayloadRef> Create(ContentId cid, long sizeBytes)
    {
        if (sizeBytes < 0)
        {
            return Result.Failure<PayloadRef>(ProvenanceErrors.Protocol.PayloadSizeNegative);
        }

        return Result.Success(new PayloadRef(cid, sizeBytes));
    }
}