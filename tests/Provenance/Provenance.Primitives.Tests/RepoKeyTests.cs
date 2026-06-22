using FluentAssertions;

using Provenance.Primitives;

using Xunit;

using static Provenance.Primitives.Errors.ProvenanceErrors.Identity;

namespace Provenance.Primitives.Tests;

/// <summary>Tests for RepoKey normalization and security validation.</summary>
[Trait("Category", "Unit")]
[Trait("Area", "Core")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class RepoKeyTests
{
    [Fact(DisplayName = "Create Valid RepoKey Should Normalize")]
    public void CreateValidRepoKeyShouldNormalize()
    {
        var repo = RepoKey.Create("GitHub.COM", "Owner_1", "Repo.Name-2");
        repo.IsSuccess.Should().BeTrue();
        repo.Value.ToString().Should().Be("github.com/owner_1/repo.name-2");
    }

    [Fact(DisplayName = "Create RepoKey with Maximum Lengths Should Succeed")]
    public void CreateRepoKeyWithMaximumLengthsShouldSucceed()
    {
        var segment = new string('a', 128);
        RepoKey.Create(segment, segment, segment).IsSuccess.Should().BeTrue();
    }

    [Fact(DisplayName = "Create RepoKey with Too Long Segment Should Fail")]
    public void CreateRepoKeyWithTooLongSegmentShouldFail() =>
        RepoKey.Create(new string('a', 129), "o", "n").IsSuccess.Should().BeFalse();

    [Theory(DisplayName = "Create RepoKey with Blank Segments Should Fail")]
    [InlineData(null, "o", "n", "BlankHost")]
    [InlineData("h", null, "n", "BlankOwner")]
    [InlineData("h", "o", null, "BlankName")]
    public void CreateRepoKeyWithBlankSegmentsShouldFail(string? host, string? owner, string? name, string _)
    {
        RepoKey.Create(host!, owner!, name!).IsSuccess.Should().BeFalse();
    }

    [Theory(DisplayName = "Create RepoKey with Security Violations Should Fail")]
    [InlineData("h..", "o", "n")]
    [InlineData("h/", "o", "n")]
    [InlineData("h", "o\\", "n")]
    [InlineData("h", "o", "n$")]
    public void CreateRepoKeyWithSecurityViolationsShouldFail(string host, string owner, string name) =>
        RepoKey.Create(host, owner, name).Error.Code.Should().Be(SecurityViolation.Code);

    [Fact(DisplayName = "Equality Is Normalized")]
    public void EqualityIsNormalized()
    {
        var key1 = RepoKey.Create("GITHUB.COM", "Owner", "Repo").Value;
        var key2 = RepoKey.Create("github.com", "owner", "repo").Value;
        (key1 == key2).Should().BeTrue();
    }
}