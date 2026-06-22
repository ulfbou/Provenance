using FluentAssertions;

namespace Provenance.Integrity.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Crypto")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class Sha256ContentHasherTests
{
    readonly Sha256ContentHasher _sut = new();

    [Theory(DisplayName = "HashAsync With Known Vectors Should Match")]
    [InlineData("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
    [InlineData("abc", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData("hello", "2cf24dba5fb0a30e26e83b2ac5b9e29e1b161e5c1fa7425e73043362938b9824")]
    public async Task HashAsyncWithKnownVectorsShouldMatch(string input, string expectedHex)
    {
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input));
        var result = await _sut.HashAsync(stream, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.HexValue.Should().Be(expectedHex);
        result.Value.Algorithm.Should().Be("sha256");
    }

    [Fact(DisplayName = "HashAsync With Empty Stream Should Return Empty Hash")]
    public async Task HashAsyncWithEmptyStreamShouldReturnEmptyHash()
    {
        using var stream = new MemoryStream();
        var result = await _sut.HashAsync(stream, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.BytesProcessed.Should().Be(0);
    }

    [Fact(DisplayName = "HashAsync Null Source Should Fail")]
    public async Task HashAsyncNullSourceShouldFail()
    {
        var result = await _sut.HashAsync(null!, null, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("provenance.integrity.null_stream");
    }

    [Fact(DisplayName = "HashAsync NonReadable Stream Should Fail")]
    public async Task HashAsyncNonReadableStreamShouldFail()
    {
        using var stream = new NonReadableStream();
        var result = await _sut.HashAsync(stream, null, CancellationToken.None);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("provenance.integrity.stream_not_readable");
    }

    [Fact(DisplayName = "HashAsync With CopyTo Copies Bytes Should Succeed")]
    public async Task HashAsyncWithCopyToCopiesBytesShouldSucceed()
    {
        using var source = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        using var copy = new MemoryStream();

        var result = await _sut.HashAsync(source, copy, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        copy.ToArray().Should().Equal(1, 2, 3, 4);
        result.Value.BytesProcessed.Should().Be(4);
    }

    [Fact(DisplayName = "HashAsync Cancelled Should Fail")]
    public async Task HashAsyncCancelledShouldFail()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        using var stream = new SlowStream();

        var result = await _sut.HashAsync(stream, null, cts.Token);
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("provenance.integrity.operation_cancelled");
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
