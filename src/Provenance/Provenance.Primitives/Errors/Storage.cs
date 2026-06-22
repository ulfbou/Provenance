using Dx.Domain.Errors;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Defines errors related to storage contracts.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These errors are used by storage adapters and persistence workflows when a request cannot be completed safely.
    /// </para>
    /// <para>
    /// Error codes are stable contracts. Messages remain safe to log and intentionally omit raw caller input and paths.
    /// </para>
    /// </remarks>
    public static partial class Storage
    {
        /// <summary>
        /// Defines stable error codes for storage-related failures.
        /// </summary>
        public static class Codes
        {
            /// <summary>Returned when a put stream is <see langword="null"/>.</summary>
            public const string PutStreamNull = "provenance.storage.put_stream_null";

            /// <summary>Returned when a commit write operation fails.</summary>
            public const string CommitWriteFailed = "provenance.storage.commit_write_failed";

            /// <summary>Returned when requested content cannot be found.</summary>
            public const string FileNotFound = "provenance.storage.file_not_found";

            /// <summary>Returned when a put stream does not support reading.</summary>
            public const string PutStreamUnreadable = "provenance.storage.put_stream_unreadable";
        }

        /// <summary>
        /// Returned when a content put stream is <see langword="null"/>.
        /// </summary>
        public static readonly DomainError PutStreamNull =
            Create(Codes.PutStreamNull, "Put stream cannot be null.");

        /// <summary>
        /// Returned when committing a storage write fails.
        /// </summary>
        public static readonly DomainError CommitWriteFailed =
            Create(Codes.CommitWriteFailed, "Commit write failed.");

        /// <summary>
        /// Returned when requested content cannot be found.
        /// </summary>
        public static readonly DomainError FileNotFound =
            Create(Codes.FileNotFound, "The specified content address could not be matched on disk.");

        /// <summary>
        /// Returned when a content put stream does not support reading.
        /// </summary>
        public static readonly DomainError PutStreamUnreadable =
            Create(Codes.PutStreamUnreadable, "The source stream does not support read operations.");
    }
}