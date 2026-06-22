using FluentAssertions;

using Provenance.Primitives;

namespace Provenance.Integrity.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Integrity")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class MerkleTreeBuilderTests
{
    static ContentId Cid(char c) => ContentId.Create("sha256", new string(c, 64)).Value;

    [Fact(DisplayName = "ComputeRoot Null Input Should Fail")]
    public void NullInput()
    {
        var root = MerkleTreeBuilder.ComputeRoot(null!);
        root.IsFailure.Should().BeTrue();
        root.Error.Code.Should().Be("provenance.integrity.merkle_null_input");
    }

    [Fact(DisplayName = "ComputeRoot Empty Should Fail")]
    public void Empty()
    {
        var root = MerkleTreeBuilder.ComputeRoot(Array.Empty<ContentId>());
        root.IsFailure.Should().BeTrue();
        root.Error.Code.Should().Be("provenance.integrity.merkle_empty_run");
    }

    [Fact(DisplayName = "ComputeRoot Single Leaf Returns Leaf Hash")]
    public void SingleLeaf()
    {
        var root = MerkleTreeBuilder.ComputeRoot(new[] { Cid('a') });
        root.IsSuccess.Should().BeTrue();
        root.Value.ToString().Should().StartWith("sha256:");
    }

    [Fact(DisplayName = "ComputeRoot Two Leaves Is Deterministic")]
    public void TwoLeaves()
    {
        var leaves = new[] { Cid('a'), Cid('b') };
        var r1 = MerkleTreeBuilder.ComputeRoot(leaves);
        var r2 = MerkleTreeBuilder.ComputeRoot(leaves);
        r1.Value.Should().Be(r2.Value);
    }

    [Fact(DisplayName = "ComputeRoot Three Leaves Duplicates Odd")]
    public void ThreeLeaves()
    {
        var leaves = new[] { Cid('a'), Cid('b'), Cid('c') };
        var root = MerkleTreeBuilder.ComputeRoot(leaves);
        root.IsSuccess.Should().BeTrue();
    }

    [Fact(DisplayName = "ComputeRoot Unsorted Input Sorts")]
    public void Unsorted()
    {
        // use valid hex characters (a-f) so ContentId.Create succeeds
        var unsorted = new[] { Cid('f'), Cid('a'), Cid('d') };
        var sorted = new[] { Cid('a'), Cid('d'), Cid('f') };

        var r1 = MerkleTreeBuilder.ComputeRoot(unsorted, preSorted: false);
        var r2 = MerkleTreeBuilder.ComputeRoot(sorted, preSorted: true);

        r1.IsSuccess.Should().BeTrue();
        r2.IsSuccess.Should().BeTrue();
        r1.Value.Should().Be(r2.Value);
    }

    [Fact(DisplayName = "ComputeRoot PreSorted Skips Sort")]
    public void PreSorted()
    {
        var leaves = new[] { Cid('a'), Cid('b'), Cid('c') };
        var root = MerkleTreeBuilder.ComputeRoot(leaves, preSorted: true);
        root.IsSuccess.Should().BeTrue();
    }

    [Fact(DisplayName = "ComputeRoot With Default ContentId Should Fail")]
    public void DefaultContentId()
    {
        var root = MerkleTreeBuilder.ComputeRoot(new[] { default(ContentId), Cid('a') });
        root.IsFailure.Should().BeTrue();
        root.Error.Code.Should().Be("provenance.integrity.merkle_invalid_leaf");
    }
}
