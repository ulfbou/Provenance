using FluentAssertions;

using static Provenance.Primitives.Errors.ProvenanceErrors.Protocol;

namespace Provenance.Protocol.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Protocol")]
[Trait("Component", "VersionVector")]
[Trait("Version", "v0.1.0-alpha")]
[Trait("Priority", "P0")]
public sealed class VersionVectorTests
{
    [Theory(DisplayName = "Create should succeed when sequence and generation are valid")]
    [InlineData(1, 1, null)]
    [InlineData(5, 2, "abc")]
    [InlineData(long.MaxValue, int.MaxValue, "max")]
    public void Create_ShouldSucceed_WhenSequenceAndGenerationAreValid(
        long sequenceNumber,
        int generation,
        string? predecessorFingerprint)
    {
        var result = VersionVector.Create(sequenceNumber, generation, predecessorFingerprint);

        result.IsSuccess.Should().BeTrue();
        result.Value.SequenceNumber.Should().Be(sequenceNumber);
        result.Value.Generation.Should().Be(generation);
        result.Value.PredecessorFingerprint.Should().Be(predecessorFingerprint);
    }

    [Theory(DisplayName = "Create should fail with invalid sequence error when sequence is less than one")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    public void Create_ShouldFailWithInvalidSequenceError_WhenSequenceIsLessThanOne(long sequenceNumber)
    {
        var result = VersionVector.Create(sequenceNumber, 1);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(VersionSequenceInvalid.Code);
    }

    [Theory(DisplayName = "Create should fail with invalid generation error when generation is less than one")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Create_ShouldFailWithInvalidGenerationError_WhenGenerationIsLessThanOne(int generation)
    {
        var result = VersionVector.Create(1, generation);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(VersionGenerationInvalid.Code);
    }

    [Fact(DisplayName = "Equality should be value based when all fields match")]
    public void Equality_ShouldBeValueBased_WhenAllFieldsMatch()
    {
        var first = VersionVector.Create(5, 2, "abc").Value;
        var second = VersionVector.Create(5, 2, "abc").Value;
        var third = VersionVector.Create(5, 2, "different").Value;

        first.Should().Be(second);
        (first == second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());

        first.Should().NotBe(third);
        (first != third).Should().BeTrue();
    }
}