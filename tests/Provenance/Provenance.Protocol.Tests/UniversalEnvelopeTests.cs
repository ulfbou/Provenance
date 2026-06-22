using FluentAssertions;

using static Provenance.Primitives.Errors.ProvenanceErrors.Protocol;

namespace Provenance.Protocol.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Protocol")]
[Trait("Component", "UniversalEnvelope")]
[Trait("Version", "v0.1.0-alpha")]
[Trait("Priority", "P0")]
public sealed class UniversalEnvelopeTests
{
    [Fact(DisplayName = "Create should succeed when envelope is valid and evidence is omitted")]
    public void Create_ShouldSucceed_WhenEnvelopeIsValidAndEvidenceIsOmitted()
    {
        var result = UniversalEnvelope.Create(
            2,
            ProtocolTestData.CollectedAt,
            ProtocolTestData.SourceSystem,
            ProtocolTestData.Tenant,
            ProtocolTestData.Repo,
            ProtocolTestData.Stream,
            ProtocolTestData.ObjectId,
            ProtocolTestData.Version,
            ProtocolTestData.Payload,
            ProtocolTestData.ProvenanceWithoutIds);

        result.IsSuccess.Should().BeTrue();
        result.Value.SchemaVersion.Should().Be(2);
        result.Value.CollectedAt.Should().Be(ProtocolTestData.CollectedAt);
        result.Value.SourceSystem.Should().Be(ProtocolTestData.SourceSystem);
        result.Value.Tenant.Should().Be(ProtocolTestData.Tenant);
        result.Value.Repo.Should().Be(ProtocolTestData.Repo);
        result.Value.Stream.Should().Be(ProtocolTestData.Stream);
        result.Value.ObjectId.Should().Be(ProtocolTestData.ObjectId);
        result.Value.Version.Should().Be(ProtocolTestData.Version);
        result.Value.Payload.Should().Be(ProtocolTestData.Payload);
        result.Value.Provenance.Should().Be(ProtocolTestData.ProvenanceWithoutIds);
        result.Value.Evidence.Should().BeNull();
    }

    [Fact(DisplayName = "Create should succeed when envelope is valid and evidence is supplied")]
    public void Create_ShouldSucceed_WhenEnvelopeIsValidAndEvidenceIsSupplied()
    {
        var result = UniversalEnvelope.Create(
            2,
            ProtocolTestData.CollectedAt,
            ProtocolTestData.SourceSystem,
            ProtocolTestData.Tenant,
            ProtocolTestData.Repo,
            ProtocolTestData.Stream,
            ProtocolTestData.ObjectId,
            ProtocolTestData.Version,
            ProtocolTestData.Payload,
            ProtocolTestData.ProvenanceWithIds,
            ProtocolTestData.Evidence);

        result.IsSuccess.Should().BeTrue();
        result.Value.Evidence.Should().Be(ProtocolTestData.Evidence);
    }

    [Theory(DisplayName = "Create should fail with invalid schema version error when schema version is less than one")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldFailWithInvalidSchemaVersionError_WhenSchemaVersionIsLessThanOne(int schemaVersion)
    {
        var result = UniversalEnvelope.Create(
            schemaVersion,
            ProtocolTestData.CollectedAt,
            ProtocolTestData.SourceSystem,
            ProtocolTestData.Tenant,
            ProtocolTestData.Repo,
            ProtocolTestData.Stream,
            ProtocolTestData.ObjectId,
            ProtocolTestData.Version,
            ProtocolTestData.Payload,
            ProtocolTestData.ProvenanceWithoutIds);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EnvelopeSchemaVersionInvalid.Code);
    }

    [Fact(DisplayName = "Equality should be value based when all envelope fields match")]
    public void Equality_ShouldBeValueBased_WhenAllEnvelopeFieldsMatch()
    {
        var first = ProtocolTestData.ValidEnvelopeWithEvidence();
        var second = ProtocolTestData.ValidEnvelopeWithEvidence();
        var third = ProtocolTestData.ValidEnvelopeWithoutEvidence();

        first.Should().Be(second);
        (first == second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());

        first.Should().NotBe(third);
        (first != third).Should().BeTrue();
    }
}