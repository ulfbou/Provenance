using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

public interface IContentHasher
{
    string Algorithm { get; }
    ValueTask<Result<HashResult>> HashAsync(Stream source, Stream? copyTo, CancellationToken ct);
}

public sealed class Sha256ContentHasher : IContentHasher
{
    public string Algorithm => "sha256";

    public async ValueTask<Result<HashResult>> HashAsync(Stream source, Stream? copyTo, CancellationToken ct)
    {
        if (source is null)
        {
            return Result.Failure<HashResult>(ProvenanceErrors.Integrity.NullStream);
        }

        if (!source.CanRead)
        {
            return Result.Failure<HashResult>(ProvenanceErrors.Integrity.StreamNotReadable);
        }

        if (copyTo is not null && !copyTo.CanWrite)
        {
            return Result.Failure<HashResult>(ProvenanceErrors.Integrity.StreamNotWritable);
        }

        try
        {
            using var sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            var buffer = new byte[8192];
            long total = 0;
            int read;

            while ((read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), ct).ConfigureAwait(false)) > 0)
            {
                ct.ThrowIfCancellationRequested();
                sha.AppendData(buffer.AsSpan(0, read));
                
                if (copyTo != null)
                {
                    await copyTo.WriteAsync(buffer.AsMemory(0, read), ct).ConfigureAwait(false);
                }

                total += read;
            }

            var hex = Convert.ToHexString(sha.GetHashAndReset()).ToLowerInvariant();
            return HashResult.Create(Algorithm, hex, total);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<HashResult>(ProvenanceErrors.Integrity.OperationCancelled);
        }
        catch (IOException ex)
        {
            return Result.Failure<HashResult>(ProvenanceErrors.Integrity.ReadFailed.WithDetail(ex.Message));
        }
    }
}
