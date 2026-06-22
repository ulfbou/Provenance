using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using NodaTime;

using static Dx.Domain.Dx;

namespace Provenance.Protocol;

/// <summary>
/// Invariant-governed domain-level implementation of the structural substrate.
/// </summary>
public sealed record UniversalEnvelope
{
    public int SchemaVersion { get; init; } = 2;
    public Instant CollectedAt { get; init; }
    public SourceSystemId SourceSystem { get; init; }
    public TenantId Tenant { get; init; }
    public RepoKey Repo { get; init; }
    public StreamId Stream { get; init; }
    public ObjectId ObjectId { get; init; }
    public VersionVector Version { get; init; }
    public PayloadRef Payload { get; init; } = null!;
    public ProvenanceData Provenance { get; init; }
    public EvidenceChainRef? Evidence { get; init; }

    private UniversalEnvelope(
        int schemaVersion,
        Instant collectedAt,
        SourceSystemId sourceSystem,
        TenantId tenant,
        RepoKey repo,
        StreamId stream,
        ObjectId objectId,
        VersionVector version,
        PayloadRef payload,
        ProvenanceData provenance,
        EvidenceChainRef? evidence)
    {
        SchemaVersion = schemaVersion;
        CollectedAt = collectedAt;
        SourceSystem = sourceSystem;
        Tenant = tenant;
        Repo = repo;
        Stream = stream;
        ObjectId = objectId;
        Version = version;
        Payload = payload;
        Provenance = provenance;
        Evidence = evidence;
    }

    public static Result<UniversalEnvelope> Create(
        int schemaVersion,
        Instant collectedAt,
        SourceSystemId sourceSystem,
        TenantId tenant,
        RepoKey repo,
        StreamId stream,
        ObjectId objectId,
        VersionVector version,
        PayloadRef payload,
        ProvenanceData provenance,
        EvidenceChainRef? evidence = null)
    {
        if (schemaVersion < 1)
            return Result.Failure<UniversalEnvelope>(ProvenanceErrors.Protocol.EnvelopeSchemaVersionInvalid);

        if (version is null)
            return Result.Failure<UniversalEnvelope>(ProvenanceErrors.Protocol.EnvelopeVersionNull);

        if (payload is null)
            return Result.Failure<UniversalEnvelope>(ProvenanceErrors.Protocol.EnvelopePayloadNull);

        if (provenance is null)
            return Result.Failure<UniversalEnvelope>(ProvenanceErrors.Protocol.EnvelopeProvenanceNull);

        return Result.Success(new UniversalEnvelope(schemaVersion, collectedAt, sourceSystem, tenant, repo, stream, objectId, version, payload, provenance, evidence));
    }
}