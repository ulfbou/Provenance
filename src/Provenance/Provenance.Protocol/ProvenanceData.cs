using Dx.Domain;
using Dx.Domain.Primitives;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using static Dx.Domain.Dx;

namespace Provenance.Protocol;

/// <summary>
/// Represents provenance metadata associated with a protocol envelope.
/// </summary>
/// <remarks>
/// <para>
/// Provenance data carries optional correlation, trace, and collection metadata for diagnostics and evidence tracking.
/// </para>
/// <para>
/// The type intentionally uses Dx.Domain primitive identifiers internally while protocol DTOs collapse these values to strings.
/// </para>
/// </remarks>
public sealed record ProvenanceData
{
    /// <summary>
    /// Gets the optional correlation identifier.
    /// </summary>
    /// <value>
    /// A parsed <see cref="CorrelationId"/>, or <see langword="null"/> when no correlation identifier is present.
    /// </value>
    public CorrelationId? CorrelationId { get; init; }

    /// <summary>
    /// Gets the optional trace identifier.
    /// </summary>
    /// <value>
    /// A parsed <see cref="TraceId"/>, or <see langword="null"/> when no trace identifier is present.
    /// </value>
    public TraceId? TraceId { get; init; }

    /// <summary>
    /// Gets the optional collection actor or tool identifier.
    /// </summary>
    /// <value>
    /// The value supplied by the collector, or <see langword="null"/> when unspecified.
    /// </value>
    public string? CollectedBy { get; init; }

    private ProvenanceData(
        CorrelationId? correlationId,
        TraceId? traceId,
        string? collectedBy)
    {
        CorrelationId = correlationId;
        TraceId = traceId;
        CollectedBy = collectedBy;
    }

    /// <summary>
    /// Creates provenance metadata.
    /// </summary>
    /// <param name="correlationId">
    /// Optional parsed correlation identifier.
    /// </param>
    /// <param name="traceId">
    /// Optional parsed trace identifier.
    /// </param>
    /// <param name="collectedBy">
    /// Optional collection actor or tool identifier.
    /// </param>
    /// <returns>
    /// A successful result containing <see cref="ProvenanceData"/>.
    /// </returns>
    public static Result<ProvenanceData> Create(
        CorrelationId? correlationId,
        TraceId? traceId,
        string? collectedBy)
    {
        return Result.Success(new ProvenanceData(correlationId, traceId, collectedBy));
    }
}