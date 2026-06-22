using Dx.Domain.Errors;

using FluentAssertions;

using Provenance.Primitives;

using Xunit;

using static Provenance.Primitives.Errors.ProvenanceErrors.Identity;

namespace Provenance.Primitives.Tests;

/// <summary>
/// Tests for identity-related value objects: StreamId, ObjectId, RunId, MerkleRoot, SourceSystemId.
/// </summary>
[Trait("Category", "Unit")]
[Trait("Area", "Core")]
[Trait("Component", "Identity")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class IdentityValueObjectsTests
{
    // <summary>Creates valid StreamIds and normalizes them to lowercase.</summary>
    [Theory(DisplayName = "Create Valid StreamId Should Succeed")]
    [InlineData("ab")]
    [InlineData("a0_")]
    [InlineData("stream_01")]
    public void CreateValidStreamIdShouldSucceed(string streamId) => StreamId.Create(streamId).IsSuccess.Should().BeTrue();

    /// <summary>Rejects StreamIds with invalid characters or formats.</summary>
    [Theory(DisplayName = "Create Invalid StreamId Should Fail")]
    [InlineData("a")]
    [InlineData("has-hyphen")]
    [InlineData("has.dot")]
    public void CreateInvalidStreamIdShouldFail(string streamdId) => StreamId.Create(streamdId).Error.Code.Should().Be(SecurityViolation.Code);

    /// <summary>Normalizes StreamIds to lowercase and trims whitespace.</summary>
    [Fact(DisplayName = "Create StreamId Should Normalize")]
    public void CreateStreamIdShouldNormalize() =>
        StreamId.Create("  STREAM_01  ").Value.Value.Should().Be("stream_01");

    /// <summary>Rejects blank or whitespace StreamIds.</summary>
    [Fact(DisplayName = "Create Blank StreamId Should Fail")]
    public void CreateBlankStreamIdShouldFail() => StreamId.Create(" ").Error.Code.Should().Be(BlankStream.Code);

    [Fact(DisplayName = "Create StreamId Should Respect Boundaries")]
    public void CreateStreamIdShouldRespectBoundaries()
    {
        StreamId.Create("ab").IsSuccess.Should().BeTrue();
        StreamId.Create(new string('a', 64)).IsSuccess.Should().BeTrue();
        StreamId.Create("a").IsSuccess.Should().BeFalse();
        StreamId.Create(new string('a', 65)).IsSuccess.Should().BeFalse();
    }

    /// <summary>Creates valid ObjectIds and allows a wide range of characters.</summary>
    [Theory(DisplayName = "Create Valid ObjectId Should Succeed")]
    [InlineData("obj-1")]
    [InlineData("path/to:file_v2.ext")]
    [InlineData("ABC123")]
    public void CreateValidObjectIdShouldSucceed(string streamdId) => ObjectId.Create(streamdId).IsSuccess.Should().BeTrue();

    /// <summary>Rejects ObjectIds with path traversal or other security issues.</summary>
    [Theory(DisplayName = "Create Invalid ObjectId Should Fail")]
    [InlineData("..")]
    [InlineData("a/../b")]
    [InlineData("bad space")]
    [InlineData("bad\\backslash")]
    public void CreateInvalidObjectIdShouldFail(string streamdId) => ObjectId.Create(streamdId).Error.Code.Should().Be(SecurityViolation.Code);

    [Fact(DisplayName = "Create ObjectId With Max Length Should Succeed")]
    public void CreateObjectIdWithMaxLengthShouldSucceed() =>
        ObjectId.Create(new string('a', 256)).IsSuccess.Should().BeTrue();

    [Fact(DisplayName = "Create ObjectId Should Preserve Case And Trim")]
    public void CreateObjectIdShouldPreserveCaseAndTrim() =>
        ObjectId.Create("  My/Object:1  ").Value.Value.Should().Be("My/Object:1");

    [Theory(DisplayName = "Create Invalid ObjectId Should Fail")]
    [InlineData("..")]
    [InlineData("a/../b")]
    [InlineData("bad space")]
    [InlineData("bad\\backslash")]
    public void CreateObjectIdWithInvalidCharactersShouldFail(string v) => ObjectId.Create(v).IsSuccess.Should().BeFalse();

    /// <summary>Rejects blank or whitespace ObjectIds.</summary>
    [Fact(DisplayName = "Create Blank ObjectId Should Fail")]
    public void CreateBlankObjectIdShouldFail() => ObjectId.Create(null!).Error.Code.Should().Be(BlankObjectId.Code);

    /// <summary>Creates valid RunIds and normalizes them to lowercase.</summary>
    [Fact(DisplayName = "Create RunId Should Normalize And Reject")]
    public void CreateRunIdShouldNormalizeAndReject()
    {
        RunId.Create("RUN_01").Value.Value.Should().Be("run_01");
    }

    /// <summary>Creates invalid RunIds with invalid characters or formats.</summary>
    [Theory(DisplayName = "Create Invalid RunId Should Fail")]
    [InlineData("a/b", "provenance.identity.security_violation")]
    [InlineData(" ", "provenance.identity.blank_run_id")]
    public void CreateInvalidRunIdShouldFail(string runId, string expectedCode) => RunId.Create(runId).Error.Code.Should().Be(expectedCode);

    // MerkleRoot
    [Fact(DisplayName = "Create Valid MerkleRoot Should Succeed")]
    public void CreateValidMerkleRootShouldSucceed()
    {
        var hex = new string('f', 64);
        var r = MerkleRoot.Create(hex);
        r.IsSuccess.Should().BeTrue();
        r.Value.ToString().Should().Be($"sha256:{hex}");
    }

    [Theory(DisplayName = "Create Invalid MerkleRoot Should Fail")]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("gggggggggggggggggggggggggggggggg")] // 64 g's, truly invalid
    public void CreateInvalidMerkleRootShouldFail2(string hash) =>
        MerkleRoot.Create(hash).IsSuccess.Should().BeFalse();

    [Theory(DisplayName = "Create Valid SourceSystem Should Succeed")]
    [InlineData("github.com")]
    [InlineData("gitlab.internal.corp")]
    [InlineData("ab")]
    public void CreateValidSourceSystemShouldSucceed(string sourceSystem)
    {
        SourceSystemId.Create(sourceSystem).IsSuccess.Should().BeTrue();
    }

    [Theory(DisplayName = "Create Invalid SourceSystem Should Fail")]
    [InlineData(null)]
    [InlineData("a/b")]
    [InlineData("a..b")]
    [InlineData("bad!")]
    public void CreateInvalidSourceSystemShouldFail(string? streamdId) =>
        SourceSystemId.Create(streamdId!).IsSuccess.Should().BeFalse();
}