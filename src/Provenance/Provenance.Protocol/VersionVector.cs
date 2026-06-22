using Dx.Domain;
using Dx.Domain.Primitives;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Protocol;

/// <summary>
/// Represents the version coordinates for a persisted protocol envelope.
/// </summary>
/// <remarks>
/// <para>
/// The sequence number and generation are contractual protocol values and must both be greater than or equal to one.
/// </para>
/// <para>
/// The predecessor fingerprint is optional and is carried as protocol metadata when available.
/// </para>
/// </remarks>
public sealed record VersionVector
{
    /// <summary>
    /// Gets the monotonically increasing sequence number for the version.
    /// </summary>
    /// <value>
    /// A value greater than or equal to one.
    /// </value>
    public long SequenceNumber { get; init; }

    /// <summary>
    /// Gets the version generation.
    /// </summary>
    /// <value>
    /// A value greater than or equal to one.
    /// </value>
    public int Generation { get; init; }

    /// <summary>
    /// Gets the optional predecessor fingerprint for the version.
    /// </summary>
    /// <value>
    /// A protocol-defined predecessor fingerprint, or <see langword="null"/> when no predecessor is recorded.
    /// </value>
    public string? PredecessorFingerprint { get; init; }

    private VersionVector(long sequenceNumber, int generation, string? predecessorFingerprint)
    {
        SequenceNumber = sequenceNumber;
        Generation = generation;
        PredecessorFingerprint = predecessorFingerprint;
    }

    /// <summary>
    /// Creates a validated version vector.
    /// </summary>
    /// <param name="sequenceNumber">
    /// The sequence number. Must be greater than or equal to one.
    /// </param>
    /// <param name="generation">
    /// The generation. Must be greater than or equal to one.
    /// </param>
    /// <param name="predecessorFingerprint">
    /// Optional predecessor fingerprint protocol metadata.
    /// </param>
    /// <returns>
    /// A successful result containing a <see cref="VersionVector"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.protocol.version_sequence_invalid</c></description></item>
    /// <item><description><c>provenance.protocol.version_generation_invalid</c></description></item>
    /// </list>
    /// </returns>
    public static Result<VersionVector> Create(
        long sequenceNumber,
        int generation,
        string? predecessorFingerprint = null)
    {
        if (sequenceNumber < 1)
        {
            return Result.Failure<VersionVector>(ProvenanceErrors.Protocol.VersionSequenceInvalid);
        }

        if (generation < 1)
        {
            return Result.Failure<VersionVector>(ProvenanceErrors.Protocol.VersionGenerationInvalid);
        }

        return Result.Success(new VersionVector(sequenceNumber, generation, predecessorFingerprint));
    }
}