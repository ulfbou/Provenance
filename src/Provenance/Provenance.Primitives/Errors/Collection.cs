using Dx.Domain.Errors;

using System;
using System.Collections.Generic;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Errors related to the Provenance.Collection subsystems.
    /// </summary>
    public static partial class Collection
    {
        /// <summary>
        /// Errors related to Git operations.
        /// </summary>
        public static class Git
        {
            /// <summary>
            /// Returned when the repository path is invalid.
            /// </summary>
            public static readonly DomainError InvalidRepositoryPath =
                Create(Codes.InvalidRepositoryPath, "The repository path is invalid.");

            /// <summary>
            /// Defines stable error codes for Git-related failures in collection operations.
            /// </summary>
            public static class Codes
            {
                /// <summary>Returned when a git command fails to execute successfully.</summary>
                public const string CommandFailed = "provenance.collection.git.command_failed";

                /// <summary>Returned when the git executable cannot be found.</summary>
                public const string NotFound = "provenance.collection.git.not_found";

                /// <summary>Returned when a git command exceeds its allowed execution time.</summary>
                public const string Timeout = "provenance.collection.git.timeout";

                /// <summary>Returned when a git command is cancelled, likely due to a timeout or user cancellation.</summary>
                public const string CommandCancelled = "provenance.collection.git.command_cancelled";

                /// <summary>Returned when the repository path is invalid.</summary>
                public const string InvalidRepositoryPath = "provenance.collection.git.invalid_repository_path";
            }

            /// <summary>
            /// Creates an error indicating a git command failed.
            /// </summary>
            /// <param name="command">The git arguments that were executed.</param>
            /// <param name="workingDirectory">The working directory.</param>
            /// <param name="exitCode">The process exit code.</param>
            /// <param name="stdErr">The standard error output.</param>
            /// <param name="innerError">The underlying runtime error code.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError GitCommandFailed(string command, string workingDirectory, int exitCode, string stdErr, string innerError) =>
                Create(
                    Codes.CommandFailed,
                    $"Git command 'git {command}' failed in '{workingDirectory}' with exit code {exitCode}.",
                    KeyValuePair.Create<string, object>("StdErr", stdErr ?? string.Empty));

            /// <summary>
            /// Creates an error indicating git is not installed or not found.
            /// </summary>
            /// <param name="reason">The underlying reason.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError GitNotFound(string reason) =>
                Create(
                    Codes.NotFound, 
                    "Git executable was not found.", 
                    KeyValuePair.Create<string, object>("Reason", reason ?? string.Empty));

            /// <summary>
            /// Creates an error indicating a git operation timed out.
            /// </summary>
            /// <param name="command">The git arguments.</param>
            /// <param name="timeout">The timeout duration.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError GitTimeout(string command, TimeSpan timeout) =>
                Create(
                    Codes.Timeout, 
                    $"Git command 'git {command}' timed out after {FormatSeconds(timeout)}s.", 
                    KeyValuePair.Create<string, object>("Timeout", timeout));

            /// <summary>
            /// Creates an error indicating a git command was cancelled, likely due to a timeout or user cancellation.
            /// </summary>
            /// <param name="arguments">The git arguments that were executed.</param>
            /// <param name="workingDirectory">The working directory.</param>
            /// <returns>A standardized <see cref="DomainError"/> instance.</returns>
            public static DomainError GitCommandCancelled(string arguments, string workingDirectory)
            {
                return Create(
                    Codes.CommandCancelled,
                    $"Git command was cancelled in '{workingDirectory}'.",
                    KeyValuePair.Create<string, object>("Arguments", arguments ?? "(no arguments)"),
                    KeyValuePair.Create<string, object>("WorkingDirectory", workingDirectory ?? "(no working directory)"));
            }
        }
    }
}