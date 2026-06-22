using FluentAssertions;

using Provenance.Primitives;

using static Provenance.Primitives.Errors.ProvenanceErrors.Protocol;

namespace Provenance.Protocol.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Protocol")]
[Trait("Component", "EvidenceChainRef")]
[Trait("Version", "v0.1.0-alpha")]
[Trait("Priority", "P0")]
public sealed class EvidenceChainRefTests
{
    [Theory(DisplayName = "Create should succeed when source content ID is valid and positions are non-negative")]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    [InlineData(long.MaxValue, int.MaxValue)]
    public void Create_ShouldSucceed_WhenSourceContentIdIsValidAndPositionsAreNonNegative(long byteOffset, int lineIndex)
    {
        var sourceCid = ContentId.Create("sha256", ProtocolTestData.Sha256C).Value;

        var result = EvidenceChainRef.Create(sourceCid, byteOffset, lineIndex);

        result.IsSuccess.Should().BeTrue();
        result.Value.SourceCid.Should().Be(sourceCid);
        result.Value.ByteOffset.Should().Be(byteOffset);
        result.Value.LineIndex.Should().Be(lineIndex);
    }

    [Theory(DisplayName = "Create should fail with negative byte offset error when byte offset is negative")]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    public void Create_ShouldFailWithNegativeByteOffsetError_WhenByteOffsetIsNegative(long byteOffset)
    {
        var sourceCid = ContentId.Create("sha256", ProtocolTestData.Sha256C).Value;

        var result = EvidenceChainRef.Create(sourceCid, byteOffset, 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EvidenceByteOffsetNegative.Code);
    }

    [Theory(DisplayName = "Create should fail with negative line index error when line index is negative")]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Create_ShouldFailWithNegativeLineIndexError_WhenLineIndexIsNegative(int lineIndex)
    {
        var sourceCid = ContentId.Create("sha256", ProtocolTestData.Sha256C).Value;

        var result = EvidenceChainRef.Create(sourceCid, 0, lineIndex);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EvidenceLineIndexNegative.Code);
    }

    [Fact(DisplayName = "Create should report byte offset error before line index error")]
    public void Create_ShouldReportByteOffsetErrorBeforeLineIndexError()
    {
        var sourceCid = ContentId.Create("sha256", ProtocolTestData.Sha256C).Value;

        var result = EvidenceChainRef.Create(sourceCid, -1, -1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(EvidenceByteOffsetNegative.Code);
    }

    [Fact(DisplayName = "Equality should be value based when all fields match")]
    public void Equality_ShouldBeValueBased_WhenAllFieldsMatch()
    {
        var sourceCid = ContentId.Create("sha256", ProtocolTestData.Sha256C).Value;
        var first = EvidenceChainRef.Create(sourceCid, 7, 3).Value;
        var second = EvidenceChainRef.Create(sourceCid, 7, 3).Value;
        var third = EvidenceChainRef.Create(sourceCid, 8, 3).Value;

        first.Should().Be(second);
        (first == second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());

        first.Should().NotBe(third);
        (first != third).Should().BeTrue();
    }
}