using FluentAssertions;

namespace Provenance.Integrity.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Crypto")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class UnsignedAlphaManifestSignerTests
{
    readonly UnsignedAlphaManifestSigner _sut = new();

    [Fact(DisplayName = "SignAsync Valid Payload Should Succeed")]
    public async Task ValidPayload()
    {
        var r = await _sut.SignAsync("{}", CancellationToken.None);
        r.IsSuccess.Should().BeTrue();
        r.Value.TrustLevel.Should().Be(SignatureTrustLevel.UntrustedInternalMarker);
    }

    [Theory(DisplayName = "SignAsync Blank Payload Should Fail")]
    [InlineData("")]
    [InlineData(null)]
    public async Task BlankPayload(string? payload)
    {
        var r = await _sut.SignAsync(payload!, CancellationToken.None);
        r.IsFailure.Should().BeTrue();
        r.Error.Code.Should().Be("provenance.manifest.blank_payload");
    }

    [Fact(DisplayName = "SignAsync Token Contains Hash")]
    public async Task TokenContainsHash()
    {
        var r = await _sut.SignAsync("test", CancellationToken.None);
        r.Value.Token.Should().StartWith("unsigned-v0.1.0-alpha-");
        r.Value.Token.Length.Should().BeGreaterThan(25);
    }

    [Fact(DisplayName = "SignAsync Cancelled Should Fail")]
    public async Task Cancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var r = await _sut.SignAsync("{}", cts.Token);
        r.IsFailure.Should().BeTrue();
    }

    [Fact(DisplayName = "SignAsync Deterministic For Same Payload")]
    public async Task Deterministic()
    {
        var r1 = await _sut.SignAsync("same", CancellationToken.None);
        var r2 = await _sut.SignAsync("same", CancellationToken.None);
        r1.Value.Token.Should().Be(r2.Value.Token);
    }
}