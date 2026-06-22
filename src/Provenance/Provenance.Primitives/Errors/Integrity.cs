using Dx.Domain.Errors;

using System.Collections.Generic;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Provides stable domain errors for Cryptographic identifiers, content hashing, verification, and Merkle root calculation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These errors are part of the Kernel contract. Codes must remain stable because they are used by tests,
    /// logs, diagnostics, serialized failure reports, and downstream tooling.
    /// </para>
    /// <para>
    /// Result-first Kernel code must map expected validation and operational failures to these errors instead of
    /// allowing raw exceptions to escape.
    /// </para>
    /// <para>
    /// Error messages are safe-to-log diagnostics and intentionally do not include raw caller input, exception messages,
    /// filesystem paths, payload fragments, or environment-specific details.
    /// </para>
    /// </remarks>
    public static partial class Integrity
    {
        /// <summary>
        /// Defines stable error codes for Cryptography-related errors.
        /// </summary>
        public static class Codes
        {
            /// <summary> Error code for <see langword="null"/>, empty, or whitespace algorithm values.</summary>
            public const string BlankAlgorithm = "provenance.integrity.blank_algorithm";

            /// <summary> Error code for unsupported algorithm values.</summary>
            public const string UnsupportedAlgorithm = "provenance.integrity.unsupported_algorithm";

            /// <summary> Error code for <see langword="null"/>, empty, or whitespace hexadecimal values.</summary>
            public const string BlankHexValue = "provenance.integrity.blank_hex_value";

            /// <summary> Error code for hexadecimal values with invalid length.</summary>
            public const string InvalidHexLength = "provenance.integrity.invalid_hex_length";

            /// <summary> Error code for hexadecimal values with invalid format or unsupported characters.</summary>
            public const string InvalidHexFormat = "provenance.integrity.invalid_hex_format";

            /// <summary> Error code for detected Cryptographic corruption during verification.</summary>
            public const string CorruptionDetected = "provenance.integrity.corruption_detected";

            /// <summary> Error code for incomplete verification when the verification boundary is reached.</summary>
            public const string VerificationIncomplete = "provenance.integrity.verification_incomplete";

            /// <summary> Error code for negative byte counts.</summary>
            public const string NegativeByteCount = "provenance.integrity.negative_byte_count";

            /// <summary> Error code for byte-count accumulation that exceeds the supported range.</summary>
            public const string ByteCountOverflow = "provenance.integrity.byte_count_overflow";

            /// <summary> Error code for null input collections to Merkle root calculation.</summary>
            public const string MerkleNullInput = "provenance.integrity.merkle_null_input";

            /// <summary> Error code for empty input collections to Merkle root calculation.</summary>
            public const string MerkleEmptyRun = "provenance.integrity.merkle_empty_run";

            /// <summary> Error code for invalid leaf entries in Merkle root calculation.</summary>
            public const string MerkleInvalidLeaf = "provenance.integrity.merkle_invalid_leaf";

            /// <summary> Error code for null source streams in Cryptographic operations.</summary>
            public const string NullStream = "provenance.integrity.null_stream";

            /// <summary> Error code for source streams that cannot be read during Cryptographic operations.</summary>
            public const string StreamNotReadable = "provenance.integrity.stream_not_readable";

            /// <summary> Error code for source streams that cannot be written during Cryptographic operations.</summary>
            public const string StreamNotWritable = "provenance.integrity.stream_not_writable";

            /// <summary> Error code for operations that are cancelled.</summary>
            public const string OperationCancelled = "provenance.integrity.operation_cancelled";

            /// <summary> Error code for unreadable streams during Cryptographic read operations.</summary>
            public const string ReadFailed = "provenance.integrity.read_failed";

            /// <summary> Error code for failures during Merkle root cannonicalization.</summary>
            public const string MerkleCanonicalizationFailed = "provenance.integrity.merkle_canonicalization_failed";

            /// <summary> Error code for unreadable streams during content hash operations.</summary>
            public const string HashReadFailed = "provenance.integrity.hash_read_failed";

            /// <summary> Error code for failures during content hash computation.</summary>
            public const string ComputationFailed = "provenance.integrity.hash_computation_failed";

            /// <summary> Error code for failures during content identifier canonicalization.</summary>
            public const string CanonicalizationFailed = "provenance.integrity.hash_canonicalization_failed";

            /// <summary> Error code for failures during Merkle root computation.</summary>
            public const string MerkleComputationFailed = "provenance.integrity.merkle_computation_failed";

            /// <summary> Error code for blank content values in Cryptographic operations.</summary>
            public static string BlankContent = "provenance.integrity.blank_content";
        }

        /// <summary>
        /// Gets the error used when a Cryptographic algorithm value is null, empty, or whitespace.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.blank_algorithm</c>.
        /// </value>
        public static readonly DomainError BlankAlgorithm =
            Create(Codes.BlankAlgorithm, "Algorithm cannot be blank.");

        /// <summary>
        /// Gets the error used when a Cryptographic algorithm is not supported.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.unsupported_algorithm</c>.
        /// </value>
        public static readonly DomainError UnsupportedAlgorithm =
            Create(Codes.UnsupportedAlgorithm, "Unsupported algorithm.");

        /// <summary>
        /// Gets the error used when a hexadecimal value is null, empty, or whitespace.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.blank_hex_value</c>.
        /// </value>
        public static readonly DomainError BlankHexValue =
            Create(Codes.BlankHexValue, "Hex value cannot be blank.");

        /// <summary>
        /// Gets the error used when a hexadecimal SHA-256 value does not contain exactly 64 characters.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.invalid_hex_length</c>.
        /// </value>
        public static readonly DomainError InvalidHexLength =
            Create(Codes.InvalidHexLength, "Invalid hex length.");

        /// <summary>
        /// Gets the error used when a hexadecimal SHA-256 value contains unsupported characters.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.invalid_hex_format</c>.
        /// </value>
        public static readonly DomainError InvalidHexFormat =
            Create(Codes.InvalidHexFormat, "Invalid hex format.");

        /// <summary>
        /// Gets the error used when Cryptographic verification detects corrupted content.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.corruption_detected</c>.
        /// </value>
        public static readonly DomainError CorruptionDetected =
            Create(Codes.CorruptionDetected, "Cryptographic corruption detected.");

        /// <summary>
        /// Gets the error used when verification was not completed before the verification boundary ended.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.verification_incomplete</c>.
        /// </value>
        public static readonly DomainError VerificationIncomplete =
            Create(Codes.VerificationIncomplete, "Verification incomplete.");

        /// <summary>
        /// Gets the error used when a byte count is negative.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.negative_byte_count</c>.
        /// </value>
        public static readonly DomainError NegativeByteCount =
            Create(Codes.NegativeByteCount, "Byte count cannot be negative.");

        /// <summary>
        /// Gets the error used when byte-count accumulation exceeds the supported range.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.byte_count_overflow</c>.
        /// </value>
        public static readonly DomainError ByteCountOverflow =
            Create(Codes.ByteCountOverflow, "Byte count exceeded the supported range.");

        /// <summary>
        /// Gets the error used when Merkle root calculation receives a null input collection.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.merkle_null_input</c>.
        /// </value>
        public static readonly DomainError MerkleNullInput =
            Create(Codes.MerkleNullInput, "Merkle tree entries list cannot be null.");

        /// <summary>
        /// Gets the error used when Merkle root calculation receives an empty input collection.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.merkle_empty_run</c>.
        /// </value>
        public static readonly DomainError MerkleEmptyRun =
            Create(Codes.MerkleEmptyRun, "Merkle tree requires at least one leaf entry component.");

        /// <summary>
        /// Gets the error used when Merkle root calculation receives an invalid leaf.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.merkle_invalid_leaf</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// APIs that accept created <see cref="ContentId"/> values trust the value-object invariant and do not normally
        /// return this error. The error is retained for compatibility and future raw-leaf validation boundaries.
        /// </para>
        /// </remarks>
        public static readonly DomainError MerkleInvalidLeaf =
            Create(Codes.MerkleInvalidLeaf, "Merkle tree contains an invalid leaf.");

        /// <summary>
        /// Gets the error used when a source stream is null.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.null_stream</c>.
        /// </value>
        public static readonly DomainError NullStream =
            Create(Codes.NullStream, "Source stream is null.");

        /// <summary>
        /// Gets the error used when a source stream does not support reading.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.stream_not_readable</c>.
        /// </value>
        public static readonly DomainError StreamNotReadable =
            Create(Codes.StreamNotReadable, "Source stream is not readable.");

        /// <summary>
        /// Gets the error used when a destination stream does not support writing.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.stream_not_writable</c>.
        /// </value>
        public static readonly DomainError StreamNotWritable =
            Create(Codes.StreamNotWritable, "Destination stream is not writable.");

        /// <summary>
        /// Gets the error used when an operation is cancelled.
        /// </summary>
        /// <value>
        /// <c>provenance.integrity.operation_cancelled</c>.
        /// </value>
        public static readonly DomainError OperationCancelled =
            Create(Codes.OperationCancelled, "Operation was cancelled.");

        /// <summary>
        /// Gets the error used when content is 
        /// </summary>
        public static DomainError BlankContent =
            Create(Codes.BlankContent, "Content is null.");

        /// <summary>
        /// Provides factory methods for read failure diagnostics.
        /// </summary>
        public static class ReadFailed
        {
            /// <summary>
            /// Creates a read failure error.
            /// </summary>
            /// <param name="detail">
            /// Caller-provided diagnostic detail. The value is intentionally not included in the public error message.
            /// </param>
            /// <returns>
            /// A domain error with code <c>provenance.integrity.read_failed</c>.
            /// </returns>
            public static DomainError WithDetail(string detail) =>
                Create(
                    Codes.ReadFailed,
                    "Read failed during stream processing.",
                    KeyValuePair.Create<string, object>("ValidationDetails", detail ?? string.Empty));
        }

        /// <summary>
        /// Creates the error used when a source stream cannot be read during content hashing.
        /// </summary>
        /// <param name="detail">
        /// Caller-provided diagnostic detail. The value is intentionally not included in the public error message.
        /// </param>
        /// <returns>
        /// A domain error with code <c>provenance.integrity.hash_read_failed</c>.
        /// </returns>
        public static DomainError HashReadFailed(string detail) =>
            Create(
                Codes.HashReadFailed, 
                "The source stream could not be read.", 
                KeyValuePair.Create<string, object>("ValidationDetails", detail ?? string.Empty));

        /// <summary>
        /// Creates the error used when content hashing cannot be completed.
        /// </summary>
        /// <param name="detail">
        /// Caller-provided diagnostic detail. The value is intentionally not included in the public error message.
        /// </param>
        /// <returns>
        /// A domain error with code <c>provenance.integrity.hash_computation_failed</c>.
        /// </returns>
        public static DomainError HashComputationFailed(string detail) =>
            Create(
                Codes.ComputationFailed, 
                "The content hash could not be computed.", 
                KeyValuePair.Create<string, object>("ValidationDetails", detail ?? string.Empty));

        /// <summary>
        /// Creates the error used when a computed content identifier cannot be represented canonically.
        /// </summary>
        /// <param name="details">
        /// Caller-provided diagnostic detail. The value is intentionally not included in the public error message.
        /// </param>
        /// <returns>
        /// A domain error with code <c>provenance.integrity.hash_canonicalization_failed</c>.
        /// </returns>
        public static DomainError HashCanonicalizationFailed(string details) =>
            Create(
                Codes.CanonicalizationFailed, 
                "The computed content identifier could not be represented canonically.", 
                KeyValuePair.Create<string, object>("ValidationDetails", details ?? string.Empty));

        /// <summary>
        /// Creates the error used when Merkle root calculation cannot be completed.
        /// </summary>
        /// <param name="details">
        /// Caller-provided diagnostic detail. The value is intentionally not included in the public error message.
        /// </param>
        /// <returns>
        /// A domain error with code <c>provenance.integrity.merkle_computation_failed</c>.
        /// </returns>
        public static DomainError MerkleComputationFailed(string details) =>
            Create(
                Codes.MerkleComputationFailed, 
                "The Merkle root could not be computed.", 
                KeyValuePair.Create<string, object>("ValidationDetails", details ?? string.Empty));

        /// <summary>
        /// Creates the error used when a computed Merkle root cannot be represented canonically.
        /// </summary>
        /// <param name="details">
        /// Caller-provided diagnostic detail. The value is intentionally not included in the public error message.
        /// </param>
        /// <returns>
        /// A domain error with code <c>provenance.integrity.merkle_canonicalization_failed</c>.
        /// </returns>
        public static DomainError MerkleCanonicalizationFailed(string details) =>
            Create(
                Codes.MerkleCanonicalizationFailed, 
                "The computed Merkle root could not be represented canonically.", 
                KeyValuePair.Create<string, object>("ValidationDetails", details ?? string.Empty));
    }
}