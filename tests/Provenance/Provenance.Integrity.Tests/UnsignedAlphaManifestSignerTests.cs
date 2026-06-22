using FluentAssertions;

using Provenance.Primitives.Errors;

namespace Provenance.Integrity.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Integrity")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class UnsignedAlphaManifestSignerTests
{
    readonly UnsignedAlphaManifestSigner _sut = new();

    [Fact(DisplayName = "SignAsync Should Succeed with Untrusted Internal Marker Trust Level When Valid Payload Should Succeed")]
    public async Task SignAsync_ShouldSucceedWithUntrustedInternalMarkerTrustLevel_WhenValidPayload()
    {
        var r = await _sut.SignAsync("{}", CancellationToken.None);
        r.IsSuccess.Should().BeTrue();
        r.Value.TrustLevel.Should().Be(SignatureTrustLevel.UntrustedInternalMarker);
    }

    [Theory(DisplayName = "SignAsync Should Fail With Blank Payload When Payload Is Invalid")]
    [InlineData("")]
    [InlineData(null)]
    public async Task SignAsync_ShouldFailWithBlankPayload_WhenPayloadIsInvalid(string? payload)
    {
        var r = await _sut.SignAsync(payload!, CancellationToken.None);
        r.IsFailure.Should().BeTrue();
        r.Error.Code.Should().Be(ProvenanceErrors.Manifest.Codes.BlankPayload);
    }

    [Fact(DisplayName = "SignAsync Should Include Token Hash When Payload Is Valid")]
    public async Task SignAsync_ShouldIncludeTokenHash_WhenPayloadIsValid()
    {
        var r = await _sut.SignAsync("test", CancellationToken.None);
        r.Value.Token.Should().StartWith("unsigned-v0.1.0-alpha-");
        r.Value.Token.Length.Should().BeGreaterThan(25);
    }

    [Fact(DisplayName = "SignAsync Should Fail When Cancelled")]
    public async Task SignAsync_ShouldFail_WhenCancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var r = await _sut.SignAsync("{}", cts.Token);
        r.IsFailure.Should().BeTrue();
        r.Error.Code.Should().Be(ProvenanceErrors.Manifest.Codes.OperationCancelled);
    }

    [Fact(DisplayName = "SignAsync Should Be Deterministic For Same Payload")]
    public async Task SignAsync_ShouldBeDeterministic_ForSamePayload()
    {
        var r1 = await _sut.SignAsync("same", CancellationToken.None);
        var r2 = await _sut.SignAsync("same", CancellationToken.None);
        r1.Value.Token.Should().Be(r2.Value.Token);
    }
}