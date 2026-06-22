using Dx.Domain.Primitives;

using FluentAssertions;

using Provenance.Primitives;

using NodaTime;

namespace Provenance.Protocol.Tests;

internal static class ProtocolTestData
{
    internal const string Sha256A = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    internal const string Sha256B = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
    internal const string Sha256C = "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";

    internal static readonly Instant CollectedAt = Instant.FromUtc(2024, 1, 2, 3, 4, 5).PlusNanoseconds(123456789);

    internal static SourceSystemId SourceSystem => SourceSystemId.Create("github.com").Value;
    internal static TenantId Tenant => TenantId.Create("acme").Value;
    internal static RepoKey Repo => RepoKey.Create("github.com", "acme", "repo").Value;
    internal static StreamId Stream => StreamId.Create("events").Value;
    internal static ObjectId ObjectId => ObjectId.Create("obj-1").Value;

    internal static ContentId PayloadCid => ContentId.Create("sha256", Sha256A).Value;
    internal static ContentId EvidenceCid => ContentId.Create("sha256", Sha256B).Value;

    internal static VersionVector Version => VersionVector.Create(5, 2, "predecessor").Value;
    internal static PayloadRef Payload => PayloadRef.Create(PayloadCid, 42).Value;
    internal static EvidenceChainRef Evidence => EvidenceChainRef.Create(EvidenceCid, 7, 3).Value;

    internal static ProvenanceData ProvenanceWithoutIds => ProvenanceData.Create(null, null, "collector").Value;

    internal static readonly ProvenanceData ProvenanceWithIds =
        ProvenanceData.Create(CorrelationId.New(), TraceId.New(), "collector").Value;

    internal static UniversalEnvelope ValidEnvelopeWithoutEvidence()
    {
        var result = UniversalEnvelope.Create(
            2,
            CollectedAt,
            SourceSystem,
            Tenant,
            Repo,
            Stream,
            ObjectId,
            Version,
            Payload,
            ProvenanceWithoutIds);

        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    internal static UniversalEnvelope ValidEnvelopeWithEvidence()
    {
        var result = UniversalEnvelope.Create(
            2,
            CollectedAt,
            SourceSystem,
            Tenant,
            Repo,
            Stream,
            ObjectId,
            Version,
            Payload,
            ProvenanceWithIds,
            Evidence);

        result.IsSuccess.Should().BeTrue();
        return result.Value;
    }

    internal static UniversalEnvelopeDto ValidDtoWithoutEvidence() =>
        new()
        {
            SchemaVersion = 2,
            CollectedAt = CollectedAt,
            SourceSystemId = "github.com",
            Tenant = "acme",
            RepoHost = "github.com",
            RepoOwner = "acme",
            RepoName = "repo",
            Stream = "events",
            ObjectId = "obj-1",
            Version = new VersionVectorDto(
                SequenceNumber: 5,
                Generation: 2,
                PredecessorFingerprint: "predecessor"),
            Payload = new PayloadRefDto(
                Cid: $"sha256:{Sha256A}",
                SizeBytes: 42),
            Provenance = new ProvenanceDataDto(
                CorrelationId: null,
                TraceId: null,
                CollectedBy: "collector"),
            Evidence = null
        };

    internal static UniversalEnvelopeDto ValidDtoWithEvidence() =>
        ValidDtoWithoutEvidence() with
        {
            Evidence = new EvidenceChainRefDto(
                SourceCid: $"sha256:{Sha256B}",
                ByteOffset: 7,
                LineIndex: 3)
        };
}