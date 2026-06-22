using FluentAssertions;

namespace Provenance.Integrity.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Integrity")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class ManifestSignatureTests
{
    [Fact(DisplayName = "Create Valid Should Succeed")]
    public void CreateValid()
    {
        var r = ManifestSignature.Create("token-123", SignatureTrustLevel.UntrustedInternalMarker);
        r.IsSuccess.Should().BeTrue();
        r.Value.Token.Should().Be("token-123");
    }

    [Theory(DisplayName = "Create Blank Token Should Fail")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void BlankToken(string? token)
    {
        var r = ManifestSignature.Create(token!, SignatureTrustLevel.None);
        r.IsFailure.Should().BeTrue();
        r.Error.Code.Should().Be("provenance.manifest.blank_token");
    }

    [Fact(DisplayName = "Equality Is Value Based")]
    public void Equality()
    {
        var a = ManifestSignature.Create("t", SignatureTrustLevel.None).Value;
        var b = ManifestSignature.Create("t", SignatureTrustLevel.None).Value;
        (a == b).Should().BeTrue();
    }
}
