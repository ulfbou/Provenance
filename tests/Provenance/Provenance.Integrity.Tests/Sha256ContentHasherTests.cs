using FluentAssertions;

using Provenance.Primitives.Errors;

namespace Provenance.Integrity.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Integrity")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class Sha256ContentHasherTests
{
    readonly Sha256ContentHasher _sut = new();

    [Theory(DisplayName = "ComputeHashAsync With Known Vectors Should Match")]
    [InlineData("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
    [InlineData("abc", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData("hello", "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824")]
    public async Task ComputeHashAsync_WithKnownVectors_ShouldMatch(string input, string expectedHex)
    {
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
        var result = await _sut.ComputeHashAsync(stream, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.ContentId.ToString().Should().Be($"sha256:{expectedHex}");
        result.Value.SizeBytes.Should().Be(System.Text.Encoding.UTF8.GetByteCount(input));
    }

    [Fact(DisplayName = "ComputeHashAsync With Empty Stream Should Return Empty Hash")]
    public async Task ComputeHashAsync_WithEmptyStream_ShouldReturnEmptyHash()
    {
        using var stream = new MemoryStream();
        var result = await _sut.ComputeHashAsync(stream, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.SizeBytes.Should().Be(0);
    }

    [Fact(DisplayName = "ComputeHashAsync When Source Is Null Should Fail With HashNullStream")]
    public async Task ComputeHashAsync_WhenSourceIsNull_ShouldFail_WithHashNullStream()
    {
        var result = await _sut.ComputeHashAsync(null!, null, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ProvenanceErrors.Integrity.Codes.NullStream);
    }

    [Fact(DisplayName = "ComputeHashAsync With NonReadable Stream Should Fail With StreamNotReadable")]
    public async Task ComputeHashAsync_WithNonReadableStream_ShouldFail_WithStreamNotReadable()
    {
        using var stream = new NonReadableStream();
        var result = await _sut.ComputeHashAsync(stream, null, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ProvenanceErrors.Integrity.Codes.StreamNotReadable);
    }

    [Fact(DisplayName = "ComputeHashAsync With CopyTo Copies Bytes Should Succeed")]
    public async Task ComputeHashAsync_WithCopyToCopiesBytes_ShouldSucceed()
    {
        using var source = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        using var copy = new MemoryStream();

        var result = await _sut.ComputeHashAsync(source, copy, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        copy.ToArray().Should().Equal(1, 2, 3, 4);
        result.Value.SizeBytes.Should().Be(4);
    }

    [Fact(DisplayName = "ComputeHashAsync When Cancelled Should Fail With OperationCancelled")]
    public async Task ComputeHashAsync_WhenCancelled_ShouldFail_WithOperationCancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        using var stream = new SlowStream();

        var result = await _sut.ComputeHashAsync(stream, null, cts.Token);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(ProvenanceErrors.Integrity.Codes.OperationCancelled);
    }

    sealed class NonReadableStream : MemoryStream
    {
        public override bool CanRead => false;
    }

    sealed class SlowStream : MemoryStream
    {
        public SlowStream() : base(new byte[8192]) { }
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken ct)
            => Task.Delay(1000, ct).ContinueWith(_ => 0, ct);
    }
}
