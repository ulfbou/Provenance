using FluentAssertions;

using static Provenance.Primitives.Errors.ProvenanceErrors.Protocol;

namespace Provenance.Protocol.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Protocol")]
[Trait("Component", "EnvelopeMapping")]
[Trait("Version", "v0.1.0-alpha")]
[Trait("Priority", "P0")]
public sealed class EnvelopeMappingTests
{
    [Fact(DisplayName = "ToDto should preserve all envelope values when envelope has no evidence")]
    public void ToDto_ShouldPreserveAllEnvelopeValues_WhenEnvelopeHasNoEvidence()
    {
        var envelope = ProtocolTestData.ValidEnvelopeWithoutEvidence();

        var result = envelope.ToDto();

        result.IsSuccess.Should().BeTrue();
        result.Value.SchemaVersion.Should().Be(envelope.SchemaVersion);
        result.Value.CollectedAt.Should().Be(envelope.CollectedAt);
        result.Value.SourceSystemId.Should().Be(envelope.SourceSystem.Value);
        result.Value.Tenant.Should().Be(envelope.Tenant.Value);
        result.Value.RepoHost.Should().Be(envelope.Repo.Host);
        result.Value.RepoOwner.Should().Be(envelope.Repo.Owner);
        result.Value.RepoName.Should().Be(envelope.Repo.Name);
        result.Value.Stream.Should().Be(envelope.Stream.Value);
        result.Value.ObjectId.Should().Be(envelope.ObjectId.Value);
        result.Value.Version.SequenceNumber.Should().Be(envelope.Version.SequenceNumber);
        result.Value.Version.Generation.Should().Be(envelope.Version.Generation);
        result.Value.Version.PredecessorFingerprint.Should().Be(envelope.Version.PredecessorFingerprint);
        result.Value.Payload.Cid.Should().Be(envelope.Payload.Cid.Value);
        result.Value.Payload.SizeBytes.Should().Be(envelope.Payload.SizeBytes);
        result.Value.Provenance.CollectedBy.Should().Be(envelope.Provenance.CollectedBy);
        result.Value.Evidence.Should().BeNull();
    }

    [Fact(DisplayName = "ToDto should preserve evidence when envelope has evidence")]
    public void ToDto_ShouldPreserveEvidence_WhenEnvelopeHasEvidence()
    {
        var envelope = ProtocolTestData.ValidEnvelopeWithEvidence();

        var result = envelope.ToDto();

        result.IsSuccess.Should().BeTrue();
        result.Value.Evidence.Should().NotBeNull();
        result.Value.Evidence!.SourceCid.Should().Be(envelope.Evidence!.SourceCid.ToString());
        result.Value.Evidence.ByteOffset.Should().Be(envelope.Evidence.ByteOffset);
        result.Value.Evidence.LineIndex.Should().Be(envelope.Evidence.LineIndex);
    }

    [Fact(DisplayName = "ToDomain should preserve all DTO values when DTO has no evidence")]
    public void ToDomain_ShouldPreserveAllDtoValues_WhenDtoHasNoEvidence()
    {
        var dto = ProtocolTestData.ValidDtoWithoutEvidence();

        var result = dto.ToDomain();

        result.IsSuccess.Should().BeTrue();
        result.Value.SchemaVersion.Should().Be(dto.SchemaVersion);
        result.Value.CollectedAt.Should().Be(dto.CollectedAt);
        result.Value.SourceSystem.Value.Should().Be(dto.SourceSystemId);
        result.Value.Tenant.Value.Should().Be(dto.Tenant);
        result.Value.Repo.Host.Should().Be(dto.RepoHost);
        result.Value.Repo.Owner.Should().Be(dto.RepoOwner);
        result.Value.Repo.Name.Should().Be(dto.RepoName);
        result.Value.Stream.Value.Should().Be(dto.Stream);
        result.Value.ObjectId.Value.Should().Be(dto.ObjectId);
        result.Value.Version.SequenceNumber.Should().Be(dto.Version.SequenceNumber);
        result.Value.Version.Generation.Should().Be(dto.Version.Generation);
        result.Value.Version.PredecessorFingerprint.Should().Be(dto.Version.PredecessorFingerprint);
        result.Value.Payload.Cid.Value.Should().Be(dto.Payload.Cid);
        result.Value.Payload.SizeBytes.Should().Be(dto.Payload.SizeBytes);
        result.Value.Provenance.CollectedBy.Should().Be(dto.Provenance.CollectedBy);
        result.Value.Evidence.Should().BeNull();
    }

    [Fact(DisplayName = "ToDomain should preserve evidence when DTO has evidence")]
    public void ToDomain_ShouldPreserveEvidence_WhenDtoHasEvidence()
    {
        var dto = ProtocolTestData.ValidDtoWithEvidence();

        var result = dto.ToDomain();

        result.IsSuccess.Should().BeTrue();
        result.Value.Evidence.Should().NotBeNull();
        result.Value.Evidence!.SourceCid.Value.Should().Be(dto.Evidence!.SourceCid);
        result.Value.Evidence.ByteOffset.Should().Be(dto.Evidence.ByteOffset);
        result.Value.Evidence.LineIndex.Should().Be(dto.Evidence.LineIndex);
    }

    [Fact(DisplayName = "Envelope should roundtrip from domain to DTO and back")]
    public void Envelope_ShouldRoundtrip_FromDomainToDtoAndBack()
    {
        var envelope = ProtocolTestData.ValidEnvelopeWithEvidence();

        var dtoResult = envelope.ToDto();

        dtoResult.IsSuccess.Should().BeTrue();

        var domainResult = dtoResult.Value.ToDomain();

        domainResult.IsSuccess.Should().BeTrue();
        domainResult.Value.Should().Be(envelope);
    }

    [Fact(DisplayName = "ToDto should fail with invalid DTO error when envelope schema version is invalid")]
    public void ToDto_ShouldFailWithInvalidDtoError_WhenEnvelopeSchemaVersionIsInvalid()
    {
        var envelope = ProtocolTestData.ValidEnvelopeWithoutEvidence() with
        {
            SchemaVersion = 0
        };

        var result = envelope.ToDto();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeInvalidDto("").Code);
        var expectedError = EnvelopeSchemaVersionInvalid.Code;
        result.Error.Metadata.Should().ContainKey("ValidationDetails");
        result.Error.Metadata["ValidationDetails"]
            .Should()
            .BeOfType<string>()
            .Which
            .Should()
            .Contain(expectedError);
    }

    [Fact(DisplayName = "ToDomain should fail with invalid DTO error and aggregate errors when DTO has invalid required fields")]
    public void ToDomain_ShouldFailWithInvalidDtoErrorAndAggregateErrors_WhenDtoHasInvalidRequiredFields()
    {
        var dto = ProtocolTestData.ValidDtoWithoutEvidence() with
        {
            SourceSystemId = "bad/source",
            Tenant = "bad/tenant",
            RepoHost = "",
            Stream = "",
            ObjectId = "",
            Version = new VersionVectorDto(0, 0, null),
            Payload = new PayloadRefDto("not-a-content-id", -1),
            Provenance = null!
        };

        var result = dto.ToDomain();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeInvalidDto("").Code);
        result.Error.Message.Should().Contain("provenance.identity.invalid_source_system");
        result.Error.Message.Should().Contain("provenance.identity.security_violation");
        result.Error.Message.Should().Contain("provenance.identity.blank_host");
        result.Error.Message.Should().Contain("provenance.identity.blank_stream");
        result.Error.Message.Should().Contain("provenance.identity.blank_object_id");
        result.Error.Message.Should().Contain(VersionSequenceInvalid.Code);
        result.Error.Message.Should().Contain("provenance.integrity.invalid_hex_length");
        result.Error.Message.Should().Contain(EnvelopeProvenanceNull.Code);
    }

    [Fact(DisplayName = "ToDomain should fail with invalid DTO error when payload size is negative")]
    public void ToDomain_ShouldFailWithInvalidDtoError_WhenPayloadSizeIsNegative()
    {
        var dto = ProtocolTestData.ValidDtoWithoutEvidence() with
        {
            Payload = ProtocolTestData.ValidDtoWithoutEvidence().Payload with
            {
                SizeBytes = -1
            }
        };

        var result = dto.ToDomain();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeInvalidDto("").Code);
        result.Error.Message.Should().Contain(PayloadSizeNegative.Code);
    }

    [Fact(DisplayName = "ToDomain should fail with invalid DTO error when evidence byte offset is negative")]
    public void ToDomain_ShouldFailWithInvalidDtoError_WhenEvidenceByteOffsetIsNegative()
    {
        var dto = ProtocolTestData.ValidDtoWithEvidence() with
        {
            Evidence = ProtocolTestData.ValidDtoWithEvidence().Evidence! with
            {
                ByteOffset = -1
            }
        };

        var result = dto.ToDomain();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeInvalidDto("").Code);
        result.Error.Message.Should().Contain("provenance.protocol.evidence_byte_offset_negative");
    }

    [Fact(DisplayName = "ToDomain should fail with invalid DTO error when evidence line index is negative")]
    public void ToDomain_ShouldFailWithInvalidDtoError_WhenEvidenceLineIndexIsNegative()
    {
        var dto = ProtocolTestData.ValidDtoWithEvidence() with
        {
            Evidence = ProtocolTestData.ValidDtoWithEvidence().Evidence! with
            {
                LineIndex = -1
            }
        };

        var result = dto.ToDomain();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeInvalidDto("").Code);
        result.Error.Message.Should().Contain("provenance.protocol.evidence_line_index_negative");
    }

    [Fact(DisplayName = "ToDomain should fail with invalid DTO error when correlation ID is malformed")]
    public void ToDomain_ShouldFailWithInvalidDtoError_WhenCorrelationIdIsMalformed()
    {
        var dto = ProtocolTestData.ValidDtoWithoutEvidence() with
        {
            Provenance = new ProvenanceDataDto(
                CorrelationId: "not-a-correlation-id",
                TraceId: null,
                CollectedBy: "collector")
        };

        var result = dto.ToDomain();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeInvalidDto("").Code);
        result.Error.Message.Should().Contain("provenance.protocol.provenance_correlation_invalid");
    }

    [Fact(DisplayName = "ToDomain should fail with invalid DTO error when trace ID is malformed")]
    public void ToDomain_ShouldFailWithInvalidDtoError_WhenTraceIdIsMalformed()
    {
        var dto = ProtocolTestData.ValidDtoWithoutEvidence() with
        {
            Provenance = new ProvenanceDataDto(
                CorrelationId: null,
                TraceId: "not-a-trace-id",
                CollectedBy: "collector")
        };

        var result = dto.ToDomain();

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeInvalidDto("").Code);
        result.Error.Message.Should().Contain("provenance.protocol.provenance_trace_invalid");
    }
}