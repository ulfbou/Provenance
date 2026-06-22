using Dx.Domain.Errors;

using System.Collections.Generic;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Defines errors related to local storage infrastructure and filesystem-backed repository operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These errors are used by the local storage adapter when repository state, filesystem access,
    /// or durable commit coordination fails.
    /// </para>
    /// <para>
    /// Error codes are stable public contracts. Error messages are human-readable diagnostics and must remain
    /// safe to log; they intentionally omit raw caller input, filesystem paths, and environment-specific details.
    /// </para>
    /// </remarks>
    public static class StorageLocal
    {
        /// <summary>
        /// Defines stable error codes for local storage failures.
        /// </summary>
        public static class Codes
        {
            /// <summary>
            /// Returned when the configured local repository root directory does not exist.
            /// </summary>
            public const string RootDirectoryMissing = "provenance.storage_local.root_directory_missing";

            /// <summary>
            /// Returned when a staging block cannot be atomically moved into place.
            /// </summary>
            public const string AtomicCommitFailed = "provenance.storage_local.atomic_commit_failed";

            /// <summary>
            /// Returned when an exclusive lock cannot be acquired before the timeout expires.
            /// </summary>
            public const string LockAcquisitionTimeout = "provenance.storage_local.lock_acquisition_timeout";

            /// <summary>
            /// Returned when a physical disk or operating-system I/O operation fails.
            /// </summary>
            public const string DeviceIoFailure = "provenance.storage_local.device_io_failure";
        }

        /// <summary>
        /// Returned when the configured local repository root directory does not exist.
        /// </summary>
        /// <value>
        /// <c>provenance.storage_local.root_directory_missing</c>.
        /// </value>
        public static readonly DomainError RootDirectoryMissing =
            Create(
                Codes.RootDirectoryMissing,
                "The configured local system repository root path does not exist.");

        /// <summary>
        /// Returned when a staging block cannot be atomically moved into its destination path.
        /// </summary>
        /// <value>
        /// <c>provenance.storage_local.atomic_commit_failed</c>.
        /// </value>
        public static readonly DomainError AtomicCommitFailed =
            Create(
                Codes.AtomicCommitFailed,
                "The staging scratch block could not be atomically moved to its destination path.");

        /// <summary>
        /// Provides factory methods for errors that occur while acquiring an exclusive local-storage lock.
        /// </summary>
        public static class LockAcquisitionTimeout
        {
            /// <summary>
            /// Creates an error that indicates an exclusive lock could not be acquired within the timeout period.
            /// </summary>
            /// <param name="safeTarget">
            /// A sanitized, log-safe lock target identifier. The value is copied into structured error metadata
            /// and should not contain raw paths or sensitive content.
            /// </param>
            /// <returns>
            /// A <see cref="DomainError"/> with code <c>provenance.storage_local.lock_acquisition_timeout</c>.
            /// </returns>
            public static DomainError WithTarget(string safeTarget) =>
                Create(
                    Codes.LockAcquisitionTimeout,
                    "Could not acquire an exclusive lock within the assigned tenant scope.",
                    KeyValuePair.Create<string, object>("LockTargetKey", safeTarget ?? string.Empty));
        }

        /// <summary>
        /// Provides factory methods for local-storage failures caused by physical disk or platform I/O errors.
        /// </summary>
        public static class DeviceIoFailure
        {
            /// <summary>
            /// Creates an error that indicates a hardware, operating-system, or platform I/O failure.
            /// </summary>
            /// <param name="systemDetails">
            /// Sanitized system detail text suitable for diagnostics. The value is copied into structured error
            /// metadata and should not contain raw exception text or other sensitive content.
            /// </param>
            /// <returns>
            /// A <see cref="DomainError"/> with code <c>provenance.storage_local.device_io_failure</c>.
            /// </returns>
            public static DomainError WithSystemDetails(string systemDetails) =>
                Create(
                    Codes.DeviceIoFailure,
                    "Hardware infrastructure tracking or platform security error encountered during physical disk operations.",
                    KeyValuePair.Create<string, object>("NativeMessage", systemDetails ?? string.Empty));
        }
    }
}