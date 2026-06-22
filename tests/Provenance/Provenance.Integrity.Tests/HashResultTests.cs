using FluentAssertions;

namespace Provenance.Integrity.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Crypto")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class HashResultTests
{
    [Fact(DisplayName = "Create Valid Should Succeed")]
    public void CreateValid()
    {
        var result = HashResult.Create("sha256", new string('a', 64), 123);
        result.IsSuccess.Should().BeTrue();
        result.Value.Algorithm.Should().Be("sha256");
        result.Value.BytesProcessed.Should().Be(123);
    }

    [Theory(DisplayName = "Create With Invalid Algorithm Should Fail")]
    [InlineData("blake3")]
    [InlineData("SHA256")]
    [InlineData("")]
    public void InvalidAlgorithm(string algorithm)
    {
        var result = HashResult.Create(algorithm, new string('a', 64), 0);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("provenance.integrity.unsupported_algorithm");
    }

    [Theory(DisplayName = "Create With Invalid Hex Should Fail")]
    [InlineData("zzzz")]
    [InlineData("abc")]
    [InlineData("G" + "000000000000000000000000000000000000000000000")]
    public void InvalidHex(string hex)
    {
        var result = HashResult.Create("sha256", hex, 0);
        result.IsFailure.Should().BeTrue();
    }

    [Fact(DisplayName = "Create With Negative Bytes Should Fail")]
    public void NegativeBytes()
    {
        var result = HashResult.Create("sha256", new string('a', 64), -1);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("provenance.integrity.negative_byte_count");
    }

    [Fact(DisplayName = "Equality Is Value Based")]
    public void Equality()
    {
        var a = HashResult.Create("sha256", new string('a', 64), 1).Value;
        var b = HashResult.Create("sha256", new string('a', 64), 1).Value;
        var c = HashResult.Create("sha256", new string('b', 64), 1).Value;

        (a == b).Should().BeTrue();
        (a == c).Should().BeFalse();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
