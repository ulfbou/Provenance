using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Provenance.Integrity;

/// <summary>
/// Defines deterministic stream-first content hashing for Provenance content-addressed storage.
/// </summary>
/// <remarks>
/// <para>
/// Implementations compute a canonical content identifier from the exact bytes read from a source stream.
/// Callers must not provide a claimed content identifier.
/// </para>
/// <para>
/// Implementations must be deterministic. Identical byte sequences read in the same order must produce identical
/// content identifiers and byte counts.
/// </para>
/// <para>
/// The source stream remains owned by the caller. Implementations must not dispose, rewind, or otherwise reset it.
/// </para>
/// <para>
/// Kernel implementations are Result-first. Validation failures, cancellation, stream failures, and Cryptographic
/// failures must be mapped to stable <see cref="ProvenanceErrors.Integrity"/> errors.
/// </para>
/// <para>
/// Only SHA-256 content identifiers are supported in v0.1.0-alpha.
/// </para>
/// </remarks>
public interface IContentHasher
{
    /// <summary>
    /// Computes the content identifier and byte count for a readable source stream.
    /// </summary>
    /// <param name="source">
    /// The readable source stream. Bytes are read from the stream's current position.
    /// </param>
    /// <param name="copyTo">
    /// An optional stream to which the read bytes will be copied. Ownership remains with the caller.
    /// </param>
    /// <param name="ct">
    /// A token used to cancel the hash operation.
    /// </param>
    /// <returns>
    /// A successful result containing the computed <see cref="ContentHashResult"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integrity.hash_null_stream</c></description></item>
    /// <item><description><c>provenance.integrity.hash_unreadable_stream</c></description></item>
    /// <item><description><c>provenance.integrity.hash_cancelled</c></description></item>
    /// <item><description><c>provenance.integrity.hash_read_failed</c></description></item>
    /// <item><description><c>provenance.integrity.hash_computation_failed</c></description></item>
    /// <item><description><c>provenance.integrity.hash_canonicalization_failed</c></description></item>
    /// <item><description><c>provenance.integrity.byte_count_overflow</c></description></item>
    /// <item><description><c>provenance.integrity.negative_byte_count</c></description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The stream position is advanced as bytes are read.
    /// </remarks>
    ValueTask<Result<ContentHashResult>> ComputeHashAsync(
        Stream source,
        Stream? copyTo,
        CancellationToken ct);
}
