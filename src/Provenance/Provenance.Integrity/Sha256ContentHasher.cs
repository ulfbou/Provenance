using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>
/// Computes canonical SHA-256 content identifiers from readable streams.
/// </summary>
/// <remarks>
/// <para>
/// The resulting content identifier uses the canonical <c>sha256:&lt;64 lowercase hex&gt;</c> form.
/// </para>
/// <para>
/// The implementation reads the source stream once from its current position.
/// It does not dispose, rewind, or otherwise reset the source stream.
/// </para>
/// <para>
/// The implementation is deterministic for identical byte sequences.
/// </para>
/// <para>
/// This implementation is Result-first. It catches operational exceptions and maps them to stable
/// <see cref="Provenance.Integrity"/> errors without exposing raw exception messages.
/// </para>
/// </remarks>
public sealed class Sha256ContentHasher : IContentHasher
{
    private const int BufferSize = 81920;

    /// <summary>
    /// Computes the canonical content identifier and byte count for a readable source stream.
    /// </summary>
    /// <param name="source">
    /// The readable source stream. Bytes are read from the stream's current position, and ownership remains with the
    /// caller.
    /// </param>
    /// <param name="copyTo">
    /// An optional stream to which the read bytes will be copied. Ownership remains with the caller.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the hash operation.
    /// </param>
    /// <returns>
    /// A successful result containing a <see cref="ContentHashResult"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integration.hash_null_stream</c></description></item>
    /// <item><description><c>provenance.integration.hash_unreadable_stream</c></description></item>
    /// <item><description><c>provenance.integration.hash_cancelled</c></description></item>
    /// <item><description><c>provenance.integration.hash_read_failed</c></description></item>
    /// <item><description><c>provenance.integration.hash_computation_failed</c></description></item>
    /// <item><description><c>provenance.integration.hash_canonicalization_failed</c></description></item>
    /// <item><description><c>provenance.integration.byte_count_overflow</c></description></item>
    /// <item><description><c>provenance.integration.negative_byte_count</c></description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// The implementation reads the source stream once from its current position and does not dispose, rewind, or
    /// otherwise reset the stream.
    /// </para>
    /// <para>
    /// The result is deterministic for identical byte sequences read in the same order.
    /// </para>
    /// </remarks>
    public async ValueTask<Result<ContentHashResult>> ComputeHashAsync(
        Stream source,
        Stream? copyTo,
        CancellationToken ct)
    {
        if (source is null)
        {
            return Result.Failure<ContentHashResult>(ProvenanceErrors.Integrity.NullStream);
        }

        if (!source.CanRead)
        {
            return Result.Failure<ContentHashResult>(ProvenanceErrors.Integrity.StreamNotReadable);
        }

        if (ct.IsCancellationRequested)
        {
            return Result.Failure<ContentHashResult>(ProvenanceErrors.Integrity.OperationCancelled);
        }

        byte[]? buffer = null;

        try
        {
            buffer = ArrayPool<byte>.Shared.Rent(BufferSize);

            using IncrementalHash hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

            long sizeBytes = 0;

            while (true)
            {
                Result<int> readResult = await ReadAsync(source, buffer, ct).ConfigureAwait(false);

                if (readResult.IsFailure)
                {
                    return Result.Failure<ContentHashResult>(readResult.Error);
                }

                int read = readResult.Value;

                if (read == 0)
                {
                    break;
                }

                if (sizeBytes > long.MaxValue - read)
                {
                    return Result.Failure<ContentHashResult>(ProvenanceErrors.Integrity.ByteCountOverflow);
                }

                hasher.AppendData(buffer.AsSpan(0, read));

                if (copyTo is not null)
                {
                    await copyTo.WriteAsync(buffer.AsMemory(0, read), ct).ConfigureAwait(false);
                }

                sizeBytes += read;
            }

            byte[] hash = hasher.GetHashAndReset();
            string hex = Sha256Hex.ToLowerHex(hash);

            Result<ContentId> contentIdResult = ContentId.Parse($"sha256:{hex}");

            if (contentIdResult.IsFailure)
            {
                return Result.Failure<ContentHashResult>(
                    ProvenanceErrors.Integrity.HashCanonicalizationFailed(contentIdResult.Error.Code));
            }

            Result<ContentHashResult> result = ContentHashResult.Create(contentIdResult.Value, sizeBytes);

            if (result.IsFailure)
            {
                return Result.Failure<ContentHashResult>(result.Error);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<ContentHashResult>(ProvenanceErrors.Integrity.OperationCancelled);
        }
        catch (Exception)
        {
            return Result.Failure<ContentHashResult>(
                ProvenanceErrors.Integrity.HashComputationFailed(default!));
        }
        finally
        {
            if (buffer is not null)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    private static async ValueTask<Result<int>> ReadAsync(
        Stream source,
        byte[] buffer,
        CancellationToken ct)
    {
        try
        {
            int read = await source
                .ReadAsync(buffer.AsMemory(0, buffer.Length), ct)
                .ConfigureAwait(false);

            return Result.Success(read);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<int>(ProvenanceErrors.Integrity.OperationCancelled);
        }
        catch (Exception)
        {
            return Result.Failure<int>(
                ProvenanceErrors.Integrity.HashReadFailed(string.Empty));
        }
    }
}
