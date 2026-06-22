using FluentAssertions;

using Provenance.Primitives;

using Xunit;

using static Provenance.Primitives.Errors.ProvenanceErrors.Identity;

namespace Provenance.Primitives.Tests;

/// <summary>
/// Tenant isolation boundary tests. v0.1.0-alpha must prevent path traversal and enforce normalization.
/// </summary>
[Trait("Category", "Unit")]
[Trait("Area", "Core")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class TenantIdTests
{
    [Theory(DisplayName = "Create Valid TenantId Should Pass")]
    [InlineData("acme")]
    [InlineData("acme_corp-2")]
    [InlineData("a.b")]
    [InlineData("ab")]
    [InlineData("0-_.")]
    public void CreateValidTenantIdShouldPass(string tenant) => TenantId.Create(tenant).IsSuccess.Should().BeTrue();

    [Fact(DisplayName = "Create Valid Minimum Length Should Pass")] 
    public void CreateValidMinimumLengthShouldPass() => TenantId.Create("ab").IsSuccess.Should().BeTrue();
    
    [Fact(DisplayName = "Create Valid Maximum Length Should Pass")] 
    public void CreateValidMaximumLengthShouldPass() => TenantId.Create(new string('a', 64)).IsSuccess.Should().BeTrue();

    [Theory(DisplayName = "Create Normalizes TenantId Should Pass")]
    [InlineData("ACME", "acme")]
    [InlineData("  Acme_Corp  ", "acme_corp")]
    [InlineData("A.B-C_D", "a.b-c_d")]
    public void CreateNormalizesTenantIdShouldPass(string input, string expected) => TenantId.Create(input).Value.Value.Should().Be(expected);

    [Theory(DisplayName = "Create Blank TenantId Should Fail")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void CreateBlankTenantIdShouldFail(string? tenant) => TenantId.Create(tenant!).Error.Code.Should().Be(BlankTenant.Code);

    [Theory(DisplayName = "Create Too Short TenantId Should Fail")]
    [InlineData("a")]
    [InlineData("1")]
    [InlineData("x")]
    public void CreateTooShortTenantIdShouldFail(string tenant) => TenantId.Create(tenant).IsSuccess.Should().BeFalse();

    [Fact(DisplayName = "Create Too Long TenantId Should Fail")] 
    public void CreateTooLongTenantIdShouldFail() => TenantId.Create(new string('a', 65)).IsSuccess.Should().BeFalse();

    [Theory(DisplayName = "Create Security Violating TenantId Should Fail")]
    [InlineData("..")]
    [InlineData("a..b")]
    [InlineData("a/b")]
    [InlineData("a\\b")]
    [InlineData("../etc")]
    [InlineData("bad!char")]
    [InlineData("bad@char")]
    [InlineData("bad char")]
    [InlineData("bad$")]
    public void CreateSecurityViolatingTenantIdShouldFail(string tenant) => TenantId.Create(tenant).Error.Code.Should().Be(SecurityViolation.Code);

    [Fact(DisplayName = "Equality Is Case Insensitive After Normalization")]
    public void EqualityIsCaseInsensitiveAfterNormalization()
    {
        var a = TenantId.Create("ACME").Value;
        var b = TenantId.Create("acme").Value;
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact(DisplayName = "ToString Returns Normalized Value")]
    public void ToStringReturnsNormalizedValue() =>
        TenantId.Create("  My_Tenant-01  ").Value.ToString().Should().Be("my_tenant-01");
}