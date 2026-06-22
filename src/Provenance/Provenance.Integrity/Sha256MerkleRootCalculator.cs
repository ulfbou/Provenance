using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>
/// Computes deterministic SHA-256 Merkle roots for ordered Provenance content identifiers.
/// </summary>
/// <remarks>
/// <para>
/// Each leaf is hashed from the canonical <see cref="ContentId.Value"/> using the UTF-8 domain-separation prefix
/// <c>provenance.merkle.leaf:v1:</c>.
/// </para>
/// <para>
/// Each internal node is hashed using the UTF-8 domain-separation prefix <c>provenance.merkle.node:v1:</c>, followed by
/// the left child hash and the right child hash.
/// </para>
/// <para>
/// When a tree level has an odd number of hashes, the final hash is duplicated before computing the parent level.
/// </para>
/// <para>
/// The final root is returned as a canonical <c>sha256:&lt;64 lowercase hex&gt;</c> <see cref="MerkleRoot"/>.
/// </para>
/// <para>
/// This implementation is Result-first. It catches operational exceptions and maps them to stable
/// <see cref="ProvenanceErrors.Integrity"/> errors without exposing raw exception messages.
/// </para>
/// </remarks>
public sealed class Sha256MerkleRootCalculator : IMerkleRootCalculator
{
    private static readonly byte[] LeafPrefix = Encoding.UTF8.GetBytes("Provenance.merkle.leaf:v1:");
    private static readonly byte[] NodePrefix = Encoding.UTF8.GetBytes("Provenance.merkle.node:v1:");

    /// <summary>
    /// Computes the canonical SHA-256 Merkle root for ordered content identifiers.
    /// </summary>
    /// <param name="leaves">
    /// The ordered content identifiers that form the Merkle tree leaves.
    /// </param>
    /// <returns>
    /// A successful result containing a canonical <see cref="MerkleRoot"/>, or a failure result with one of:
    /// <list type="bullet">
    /// <item><description><c>provenance.integration.merkle_null_input</c></description></item>
    /// <item><description><c>provenance.integration.merkle_empty_run</c></description></item>
    /// <item><description><c>provenance.integration.merkle_canonicalization_failed</c></description></item>
    /// <item><description><c>provenance.integration.merkle_computation_failed</c></description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// The result is deterministic for the same ordered sequence of canonical input identifiers.
    /// </remarks>
    public Result<MerkleRoot> ComputeRoot(IReadOnlyList<ContentId> leaves)
    {
        if (leaves is null)
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.MerkleNullInput);
        }

        if (leaves.Count == 0)
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.MerkleEmptyRun);
        }

        try
        {
            List<byte[]> level = new(leaves.Count);

            for (int index = 0; index < leaves.Count; index++)
            {
                level.Add(HashLeaf(leaves[index]));
            }

            while (level.Count > 1)
            {
                List<byte[]> nextLevel = new((level.Count + 1) / 2);

                for (int index = 0; index < level.Count; index += 2)
                {
                    byte[] left = level[index];
                    byte[] right = index + 1 < level.Count
                        ? level[index + 1]
                        : left;

                    nextLevel.Add(HashNode(left, right));
                }

                level = nextLevel;
            }

            string hex = Sha256Hex.ToLowerHex(level[0]);

            Result<MerkleRoot> merkleRootResult = MerkleRoot.Create(hex);

            if (merkleRootResult.IsFailure)
            {
                return Result.Failure<MerkleRoot>(
                    ProvenanceErrors.Integrity.MerkleCanonicalizationFailed(merkleRootResult.Error.Code));
            }

            return merkleRootResult;
        }
        catch (Exception)
        {
            return Result.Failure<MerkleRoot>(
                ProvenanceErrors.Integrity.MerkleComputationFailed(string.Empty));
        }
    }

    private static byte[] HashLeaf(ContentId leaf)
    {
        byte[] value = Encoding.UTF8.GetBytes(leaf.Value);

        using IncrementalHash hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hasher.AppendData(LeafPrefix);
        hasher.AppendData(value);

        return hasher.GetHashAndReset();
    }

    private static byte[] HashNode(byte[] left, byte[] right)
    {
        using IncrementalHash hasher = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hasher.AppendData(NodePrefix);
        hasher.AppendData(left);
        hasher.AppendData(right);

        return hasher.GetHashAndReset();
    }
}
