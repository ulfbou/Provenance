using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using System.Collections.Generic;

namespace Provenance.Integrity;

/// <summary>
/// Defines deterministic Merkle root calculation for ordered Provenance content identifiers.
/// </summary>
/// <remarks>
/// <para>
/// Input order is part of the Merkle root contract. Reordering the same content identifiers produces a different root.
/// </para>
/// <para>
/// Implementations must reject null input and empty input using stable crypto errors.
/// </para>
/// <para>
/// Leaf values are created <see cref="ContentId"/> value objects and are trusted under the Provenance value-object invariant.
/// </para>
/// <para>
/// Kernel implementations are Result-first. Operational failures must be mapped to stable
/// <see cref="ProvenanceErrors.Integrity"/> errors.
/// </para>
/// <para>
/// Only SHA-256 Merkle roots are supported in v0.1.0-alpha.
/// </para>
/// </remarks>
public interface IMerkleRootCalculator
{
    /// <summary>
    /// Computes a canonical Merkle root from ordered content identifiers.
    /// </summary>
    /// <param name="leaves">
    /// The ordered content identifiers that form the Merkle tree leaves.
    /// </param>
    /// <returns>
    /// A successful result containing the computed <see cref="MerkleRoot"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integrity.merkle_null_input</c></description></item>
    /// <item><description><c>provenance.integrity.merkle_empty_run</c></description></item>
    /// <item><description><c>provenance.integrity.merkle_computation_failed</c></description></item>
    /// <item><description><c>provenance.integrity.merkle_canonicalization_failed</c></description></item>
    /// </list>
    /// </returns>
    Result<MerkleRoot> ComputeRoot(IReadOnlyList<ContentId> leaves);
}
