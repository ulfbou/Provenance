using Dx.Domain.Errors;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Error codes emitted by the provenance-preprocess phase.
    /// </summary>
    public static partial class Preprocessing
    {
        /// <summary>
        /// Defines stable error codes for failures that occur during provenance preprocessing.
        /// </summary>
        public static partial class Codes
        {
            /// <summary>Returned when an input frame is structurally invalid for preprocessing.</summary>
            public const string InvalidInputFrame = "provenance.preprocessing.invalid_input_frame";

            /// <summary>Returned when an input frame type is not supported by preprocessing.</summary>
            public const string UnsupportedFrameType = "provenance.preprocessing.unsupported_frame_type";

            /// <summary>Returned when path normalization fails.</summary>
            public const string PathNormalizationFailed = "provenance.preprocessing.path_normalization_failed";

            /// <summary>Returned when input content encoding is unsupported by the configured preprocessing policy.</summary>
            public const string UnsupportedEncoding = "provenance.preprocessing.unsupported_encoding";

            /// <summary>Returned when content normalization fails.</summary>
            public const string NormalizationFailed = "provenance.preprocessing.normalization_failed";

            /// <summary>Returned when writing staged content fails.</summary>
            public const string StagingWriteFailed = "provenance.preprocessing.staging_write_failed";

            /// <summary>Returned when independently re-hashed staged content does not match the computed content identifier.</summary>
            public const string StagedHashMismatch = "provenance.preprocessing.staged_hash_mismatch";

            /// <summary>Returned when preprocessing is cancelled.</summary>
            public const string Cancelled = "provenance.preprocessing.cancelled";
        }  

        /// <summary>
        /// Input frame was structurally invalid for preprocessing.
        /// </summary>
        public static readonly DomainError InvalidInputFrame =
            Create(Codes.InvalidInputFrame, "Invalid input frame.");

        /// <summary>
        /// Input frame type is not supported by preprocessing.
        /// </summary>
        public static readonly DomainError UnsupportedFrameType =
            Create(Codes.UnsupportedFrameType, "Unsupported frame type.");

        /// <summary>
        /// Path normalization failed.
        /// </summary>
        public static readonly DomainError PathNormalizationFailed =
            Create(Codes.PathNormalizationFailed, "Path normalization failed.");

        /// <summary>
        /// Input content encoding is unsupported by the configured preprocessing policy.
        /// </summary>
        public static readonly DomainError UnsupportedEncoding =
            Create(Codes.UnsupportedEncoding, "Unsupported encoding.");

        /// <summary>
        /// Content normalization failed.
        /// </summary>
        public static readonly DomainError NormalizationFailed =
            Create(Codes.NormalizationFailed, "Content normalization failed.");

        /// <summary>
        /// Writing staged content failed.
        /// </summary>
        public static readonly DomainError StagingWriteFailed =
            Create(Codes.StagingWriteFailed, "Staging write failed.");

        /// <summary>
        /// Independently re-hashed staged content did not match the computed content identifier.
        /// </summary>
        public static readonly DomainError StagedHashMismatch =
            Create(Codes.StagedHashMismatch, "Staged hash mismatch.");

        /// <summary>
        /// Preprocessing was cancelled.
        /// </summary>
        public static readonly DomainError Cancelled =
            Create(Codes.Cancelled, "Operation cancelled.");
    }
}