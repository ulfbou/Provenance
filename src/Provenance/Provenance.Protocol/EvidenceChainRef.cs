using Dx.Domain;
using Dx.Domain.Primitives;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Protocol;

/// <summary>
/// Represents a reference to source evidence for an envelope.
/// </summary>
/// <remarks>
/// <para>
/// Evidence references point from normalized protocol data back to source content and location metadata.
/// </para>
/// <para>
/// Byte offset and line index values must not be negative.
/// </para>
/// </remarks>
public sealed record EvidenceChainRef
{
    /// <summary>
    /// Gets the content identifier for the source evidence.
    /// </summary>
    /// <value>
    /// A validated <see cref="ContentId"/> for the source content.
    /// </value>
    public ContentId SourceCid { get; init; }

    /// <summary>
    /// Gets the byte offset in the source content.
    /// </summary>
    /// <value>
    /// A non-negative byte offset.
    /// </value>
    public long ByteOffset { get; init; }

    /// <summary>
    /// Gets the line index in the source content.
    /// </summary>
    /// <value>
    /// A non-negative line index.
    /// </value>
    public int LineIndex { get; init; }

    private EvidenceChainRef(ContentId sourceCid, long byteOffset, int lineIndex)
    {
        SourceCid = sourceCid;
        ByteOffset = byteOffset;
        LineIndex = lineIndex;
    }

    /// <summary>
    /// Creates a validated evidence-chain reference.
    /// </summary>
    /// <param name="sourceCid">
    /// The content identifier for the source evidence.
    /// </param>
    /// <param name="byteOffset">
    /// The byte offset in the source content. Must not be negative.
    /// </param>
    /// <param name="lineIndex">
    /// The line index in the source content. Must not be negative.
    /// </param>
    /// <returns>
    /// A successful result containing an <see cref="EvidenceChainRef"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.protocol.evidence_byte_offset_negative</c></description></item>
    /// <item><description><c>provenance.protocol.evidence_line_index_negative</c></description></item>
    /// </list>
    /// </returns>
    public static Result<EvidenceChainRef> Create(
        ContentId sourceCid,
        long byteOffset,
        int lineIndex)
    {
        if (byteOffset < 0)
        {
            return Result.Failure<EvidenceChainRef>(ProvenanceErrors.Protocol.EvidenceByteOffsetNegative);
        }

        if (lineIndex < 0)
        {
            return Result.Failure<EvidenceChainRef>(ProvenanceErrors.Protocol.EvidenceLineIndexNegative);
        }

        return Result.Success(new EvidenceChainRef(sourceCid, byteOffset, lineIndex));
    }
}