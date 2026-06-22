using Dx.Domain;

using FluentAssertions;

using Provenance.Primitives;

using System;

using Xunit;

using static Provenance.Primitives.Errors.ProvenanceErrors.Integrity;
using static Dx.Domain.Dx;

namespace Provenance.Primitives.Tests;

/// <summary>
/// Cryptographic identifier tests. ContentId is the trust anchor for v0.1.0-alpha.
/// Ensures SHA-256 pinning, canonical hex, and safe parsing.
/// </summary>
[Trait("Category", "Unit")]
[Trait("Area", "Core")]
[Trait("Component", "Crypto")]
[Trait("Version", "v0.1.0-alpha")]
[Trait("Priority", "P0")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class ContentIdTests
{
    private const string ValidHex = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
    private const string AllZeros = "0000000000000000000000000000000000000000000000000000000000000000";
    private const string AllFs = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

    [Theory(DisplayName = "Create Valid ContentId Should Pass")]
    [InlineData(ValidHex)]
    [InlineData(AllZeros)]
    [InlineData(AllFs)]
    public void CreateValidContentIdShouldPass(string hex)
    {
        var result = ContentId.Create("sha256", hex);
        result.IsSuccess.Should().BeTrue();
        result.Value.Algorithm.Should().Be("sha256");
        result.Value.Hex.Should().Be(hex);
        result.Value.ToString().Should().Be($"sha256:{hex}");
    }

    [Theory(DisplayName = "Create ContentId with Blank Algorithm")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void CreateBlankAlgorithm(string? algorithm) =>
        ContentId.Create(algorithm!, ValidHex).Error.Code.Should().Be(BlankAlgorithm.Code);

    [Theory(DisplayName = "Create ContentId with Unsupported Algorithm")]
    [InlineData("md5")]
    [InlineData("SHA 256")]
    [InlineData("sha1")]
    [InlineData("sha-256")]
    public void CreateUnsupported(string algorithm)
    {
        Result<ContentId> result = ContentId.Create(algorithm, ValidHex);
        result.Error.Code.Should().Be(UnsupportedAlgorithm.Code);
    }

    [Theory(DisplayName = "Create ContentId with Blank Hex")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateBlankHex(string? hex) =>
        ContentId.Create("sha256", hex!).Error.Code.Should().Be(BlankHexValue.Code);

    [Theory(DisplayName = "Create ContentId with Invalid Length")]
    [InlineData(1)]
    [InlineData(63)]
    [InlineData(65)]
    [InlineData(128)]
    public void CreateInvalidLength(int length) =>
        ContentId.Create("sha256", new string('a', length)).Error.Code.Should().Be(InvalidHexLength.Code);

    [Theory(DisplayName = "Create ContentId with Invalid Format")]
    [InlineData("g3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")] // 'g' not hex
    [InlineData("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b85-")] // trailing '-'
    [InlineData("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649 b934ca495991b7852b85")] // internal space
    public void CreateInvalidFormat(string hex) =>
        ContentId.Create("sha256", hex).Error.Code.Should().Be(InvalidHexFormat.Code);

    [Fact(DisplayName = "Create ContentId normalizes uppercase hex")]
    public void CreateNormalizesUppercase() =>
        ContentId.Create("sha256", ValidHex.ToUpperInvariant())
            .Value.Hex.Should().Be(ValidHex);

    [Fact(DisplayName = "Create ContentId trims whitespace in hex")]
    public void CreateTrimsWhitespaceInHex() =>
        ContentId.Create("sha256", " " + ValidHex + "\t")
            .IsSuccess.Should().BeTrue();

    [Theory(DisplayName = "Parse ContentId Without Prefix Assumes Sha256")]
    [InlineData("  " + ValidHex + "  ")]
    [InlineData("\t" + ValidHex + "\n")]
    public void ParseWithoutPrefixAssumesSha256(string input)
    {
        var result = ContentId.Parse(input);
        result.IsSuccess.Should().BeTrue();
        result.Value.Algorithm.Should().Be("sha256");
    }

    [Theory(DisplayName = "Parse ContentId With Prefix Normalizes")]
    [InlineData("SHA256:" + ValidHex)]
    [InlineData("sha256:" + ValidHex)]
    [InlineData("  Sha256:" + ValidHex + "  ")]
    public void ParseWithPrefixNormalizes(string input) =>
        ContentId.Parse(input).Value.ToString().Should().Be($"sha256:{ValidHex}");

    [Fact(DisplayName = "Parse ContentId Normalizes Uppercase Hex")]
    public void ParseNormalizesUppercaseHex()
    {
        var result = ContentId.Parse("SHA256:" + ValidHex.ToUpperInvariant());
        result.IsSuccess.Should().BeTrue();
        result.Value.Hex.Should().Be(ValidHex);
    }

    [Theory(DisplayName = "Parse ContentId Rejects Invalid Forms")]
    [InlineData("sha256:")]
    [InlineData("sha256:abc")]
    [InlineData("sha256:abc:def")]
    [InlineData(":")]
    public void ParseRejectsInvalidForms(string input) =>
        ContentId.Parse(input).IsFailure.Should().BeTrue();

    [Fact(DisplayName = "Parse Blank ContentId Fails")]
    public void ParseBlankFails() =>
        ContentId.Parse("   ").Error.Code.Should().Be(BlankHexValue.Code);

    [Fact(DisplayName = "ContentId Equality Is Value Based")]
    public void EqualityIsValueBased()
    {
        var a = ContentId.Create("sha256", ValidHex).Value;
        var b = ContentId.Parse("SHA256:" + ValidHex.ToUpperInvariant()).Value;
        var c = ContentId.Create("sha256", AllZeros).Value;

        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
        (a != c).Should().BeTrue();
    }

    [Fact(DisplayName = "ContentId ToString Is Canonical")]
    public void ToStringIsCanonical()
    {
        var id = ContentId.Create("sha256", ValidHex).Value;
        id.ToString().Should().Be($"sha256:{ValidHex}");
        id.Value.Should().Be(id.ToString());
    }
}