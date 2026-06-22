using Dx.Domain;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using static Dx.Domain.Dx;

namespace Provenance.Integrity;

/// <summary>
/// Provides a deterministic Merkle tree implementation for Provenance content identifiers.
/// </summary>
/// <remarks>
/// <para>
/// Each leaf is hashed from the canonical <see cref="ContentId.Value"/> using a domain
/// separation prefix of 0x00.
/// </para>
/// <para>
/// Each internal node is hashed using a domain separation prefix of 0x01, followed by
/// the left child hash and the right child hash.
/// </para>
/// <para>
/// When a tree level has an odd number of hashes, the final hash is duplicated before computing
/// the parent level.
/// </para>
/// <para>
/// The final root is returned as a canonical <c>sha256:&lt;64 lowercase
/// hex&gt;</c> <see cref="MerkleRoot"/>.
/// </para>
/// </remarks>
public static class MerkleTreeBuilder
{
    /// <summary>
    /// Computes the Merkle root for a given set of content IDs.
    /// </summary>
    /// <param name="entries">The content IDs to include in the Merkle tree.</param>
    /// <param name="preSorted">Indicates whether the entries are already sorted.</param>
    /// <returns>The Merkle root of the entries.</returns>
    public static Result<MerkleRoot> ComputeRoot(IEnumerable<ContentId> entries, bool preSorted = false)
    {
        if (entries is null)
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.MerkleNullInput);
        }

        // Validate no default ContentId (would produce "sha256:")
        if (entries.Any(e => e == default))
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.MerkleInvalidLeaf);
        }

        // 1. Deterministic ordering — only sort if caller didn't
        var orderedStrings = preSorted
            ? entries.Select(e => e.ToString())
            : entries.Select(e => e.ToString()).OrderBy(s => s, StringComparer.Ordinal);

        // 2. Leaf hashes with domain separation 0x00
        var currentLevel = orderedStrings
           .Select(s => Encoding.UTF8.GetBytes(s))
           .Select(ComputeLeafHash)
           .ToList();

        if (currentLevel.Count == 0)
        {
            return Result.Failure<MerkleRoot>(ProvenanceErrors.Integrity.MerkleEmptyRun);
        }

        // 3. Build tree with 0x01, duplicate odd nodes
        while (currentLevel.Count > 1)
        {
            var nextLevel = new List<byte[]>((currentLevel.Count + 1) / 2);

            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                var left = currentLevel[i];
                var right = i + 1 < currentLevel.Count ? currentLevel[i + 1] : left;
                nextLevel.Add(ComputeNodeHash(left, right));
            }
            
            currentLevel = nextLevel;
        }

        var rootHex = Convert.ToHexString(currentLevel[0]).ToLowerInvariant();
        return MerkleRoot.Create(rootHex);
    }

    private static byte[] ComputeLeafHash(byte[] content)
    {
        byte[] buffer = new byte[1 + content.Length];
        buffer[0] = 0x00;
        Buffer.BlockCopy(content, 0, buffer, 1, content.Length);
        return SHA256.HashData(buffer);
    }

    private static byte[] ComputeNodeHash(byte[] left, byte[] right)
    {
        byte[] buffer = new byte[1 + left.Length + right.Length];
        buffer[0] = 0x01;
        Buffer.BlockCopy(left, 0, buffer, 1, left.Length);
        Buffer.BlockCopy(right, 0, buffer, 1 + left.Length, right.Length);
        return SHA256.HashData(buffer);
    }
}
