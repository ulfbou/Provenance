using Dx.Domain.Errors;

using System;
using System.Collections.Generic;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Errors related to the Provenance.Runtime process execution subsystem.
    /// </summary>
    public static partial class Runtime
    {
        /// <summary>
        /// Errors related to process lifecycle and execution.
        /// </summary>
        public static class Process
        {
            /// <summary>
            /// Defines stable error codes for process execution failures. 
            /// </summary>
            public static class Codes
            {
                /// <summary>Returned when a process executable cannot be found.</summary>
                public const string ExecutableNotFound = "provenance.runtime.process.executable_not_found";

                /// <summary>Returned when a process fails to start.</summary>
                public const string StartFailed = "provenance.runtime.process.start_failed";

                /// <summary>Returned when a process exceeds its allowed execution time.</summary>
                public const string Timeout = "provenance.runtime.process.timeout";

                /// <summary>Returned when a process exits with a non-zero exit code.</summary>
                public const string NonZeroExitCode = "provenance.runtime.process.non_zero_exit_code";

                /// <summary>Returned when a process is terminated by the system or user.</summary>
                public const string Terminated = "provenance.runtime.process.terminated";

                /// <summary>Returned when an I/O failure occurs while reading process output streams.</summary>
                public const string StreamReadFailed = "provenance.runtime.process.stream_read_failed";

                /// <summary>Returned when a process is cancelled, likely due to a timeout or user cancellation.</summary>
                public const string CommandCancelled = "provenance.runtime.process.command_cancelled";
            }

            /// <summary>
            /// Creates an error indicating the process executable could not be found.
            /// </summary>
            /// <param name="fileName">The executable name that was not found.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError ExecutableNotFound(string fileName) =>
                Create(Codes.ExecutableNotFound, $"Process executable '{fileName}' was not found on PATH or at the specified location.");

            /// <summary>
            /// Creates an error indicating the process failed to start.
            /// </summary>
            /// <param name="fileName">The executable name.</param>
            /// <param name="reason">The underlying system reason.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError StartFailed(string fileName, string reason) =>
                Create(Codes.StartFailed, $"Failed to start process '{fileName}'.", KeyValuePair.Create<string, object>("Reason", reason));

            /// <summary>
            /// Creates an error indicating the process timed out.
            /// </summary>
            /// <param name="fileName">The executable name.</param>
            /// <param name="timeout">The timeout that elapsed.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError Timeout(string fileName, TimeSpan timeout) =>
                Create(Codes.Timeout, $"Process '{fileName}' timed out after {FormatSeconds(timeout)}s.");

            /// <summary>Returned when a process was cancelled, likely due to a timeout or user cancellation.</summary>
            public static readonly DomainError CommandCancelled =
                Create(Codes.CommandCancelled, "The process command was cancelled.");

            /// <summary>
            /// Creates an error indicating the process exited with a non-zero exit code.
            /// </summary>
            /// <param name="fileName">The executable name.</param>
            /// <param name="exitCode">The exit code returned.</param>
            /// <param name="stdErr">The captured standard error output.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError NonZeroExitCode(string fileName, int exitCode, string stdErr) =>
                Create(Codes.NonZeroExitCode, $"Process '{fileName}' exited with code {exitCode}.", KeyValuePair.Create<string, object>("StdErr", stdErr));

            /// <summary>
            /// Creates an error indicating the process was terminated.
            /// </summary>
            /// <param name="fileName">The executable name.</param>
            /// <param name="reason">The reason for termination.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError Terminated(string fileName, string reason) =>
                Create(Codes.Terminated, $"Process '{fileName}' was terminated.", KeyValuePair.Create<string, object>("Reason", reason));

            /// <summary>
            /// Creates an error indicating an I/O failure while reading process streams.
            /// </summary>
            /// <param name="fileName">The executable name.</param>
            /// <param name="reason">The underlying I/O reason.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError StreamReadFailed(string fileName, string reason) =>
                Create(Codes.StreamReadFailed, $"Failed to read output stream from process '{fileName}'.", KeyValuePair.Create<string, object>("Reason", reason));
        }
    }
}