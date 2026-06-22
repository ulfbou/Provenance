using FluentAssertions;

using Provenance.Primitives;

using static Provenance.Primitives.Errors.ProvenanceErrors.Protocol;

namespace Provenance.Protocol.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Protocol")]
[Trait("Component", "PayloadRef")]
[Trait("Version", "v0.1.0-alpha")]
[Trait("Priority", "P0")]
public sealed class PayloadRefTests
{
    [Theory(DisplayName = "Create should succeed when content ID is valid and size is non-negative")]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(long.MaxValue)]
    public void Create_ShouldSucceed_WhenContentIdIsValidAndSizeIsNonNegative(long sizeBytes)
    {
        var cid = ContentId.Create("sha256", ProtocolTestData.Sha256A).Value;

        var result = PayloadRef.Create(cid, sizeBytes);

        result.IsSuccess.Should().BeTrue();
        result.Value.Cid.Should().Be(cid);
        result.Value.SizeBytes.Should().Be(sizeBytes);
    }

    [Theory(DisplayName = "Create should fail with negative size error when size is negative")]
    [InlineData(-1)]
    [InlineData(long.MinValue)]
    public void Create_ShouldFailWithNegativeSizeError_WhenSizeIsNegative(long sizeBytes)
    {
        var cid = ContentId.Create("sha256", ProtocolTestData.Sha256A).Value;

        var result = PayloadRef.Create(cid, sizeBytes);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(PayloadSizeNegative.Code);
    }

    [Fact(DisplayName = "Equality should be value based when content ID and size match")]
    public void Equality_ShouldBeValueBased_WhenContentIdAndSizeMatch()
    {
        var cid = ContentId.Create("sha256", ProtocolTestData.Sha256A).Value;
        var first = PayloadRef.Create(cid, 42).Value;
        var second = PayloadRef.Create(cid, 42).Value;
        var third = PayloadRef.Create(cid, 43).Value;

        first.Should().Be(second);
        (first == second).Should().BeTrue();
        first.GetHashCode().Should().Be(second.GetHashCode());

        first.Should().NotBe(third);
        (first != third).Should().BeTrue();
    }
}