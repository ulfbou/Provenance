using System;

namespace Provenance.Integrity;
/// <summary>
/// Provides culture-invariant lowercase hexadecimal formatting for byte sequences used by SHA-256 Integrity operations.
/// </summary>
/// <remarks>
/// <para>
/// This helper intentionally emits lowercase hexadecimal without relying on culture-sensitive formatting.
/// </para>
/// <para>
/// The helper formats the supplied byte sequence and does not itself validate that the sequence length is exactly
/// the SHA-256 digest length. Callers canonicalize through Provenance value-object factories where required.
/// </para>
/// </remarks>
internal static class Sha256Hex
{
    private const string LowerHexAlphabet = "0123456789abcdef";

    /// <summary>
    /// Converts a byte sequence to lowercase hexadecimal.
    /// </summary>
    /// <param name="hash">
    /// The hash bytes to encode.
    /// </param>
    /// <returns>
    /// A lowercase hexadecimal string.
    /// </returns>
    internal static string ToLowerHex(ReadOnlySpan<byte> hash)
    {
        char[] chars = new char[hash.Length * 2];

        for (int index = 0; index < hash.Length; index++)
        {
            byte value = hash[index];

            chars[index * 2] = LowerHexAlphabet[value >> 4];
            chars[(index * 2) + 1] = LowerHexAlphabet[value & 0x0F];
        }

        return new string(chars);
    }
}
