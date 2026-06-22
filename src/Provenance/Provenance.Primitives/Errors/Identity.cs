using Dx.Domain.Errors;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Provides stable domain errors for identity validation, including tenant identifiers, repository identifiers, stream 
    /// identifiers, object identifiers, run identifiers, source system identifiers, and correlation identifiers.
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
    public static partial class Identity
    {
        /// <summary>Defines stable error codes for identity-related errors.</summary>
        public static class Codes
        {
            /// <summary>Error code for <see langword="null"/>, empty, or whitespace tenant identifiers.</summary>
            public const string BlankTenant = "provenance.identity.blank_tenant";

            /// <summary>Error code for identity values that violate path-safety or security constraints.</summary>
            public const string SecurityViolation = "provenance.identity.security_violation";

            /// <summary>Error code for repository host segments that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankHost = "provenance.identity.blank_host";

            /// <summary>Error code for repository owner segments that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankOwner = "provenance.identity.blank_owner";

            /// <summary>Error code for repository name segments that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankName = "provenance.identity.blank_name";

            /// <summary>Error code for stream identifiers that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankStream = "provenance.identity.blank_stream";

            /// <summary>Error code for object identifiers that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankObjectId = "provenance.identity.blank_object_id";

            /// <summary>Error code for run identifiers that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankRunId = "provenance.identity.blank_run_id";

            /// <summary>Error code for source system identifiers that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankSourceSystem = "provenance.identity.blank_source_system";
            
            /// <summary>Error code for source system identifiers that do not satisfy canonical source-system rules.</summary>
            public const string InvalidSourceSystem = "provenance.identity.invalid_source_system";

            /// <summary>Error code for correlation identifiers that are <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankCorrelationId = "provenance.identity.blank_correlation_id";
        }

        /// <summary>
        /// Returned when a tenant identifier is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankTenant =
            Create(Codes.BlankTenant, "Tenant cannot be blank.");

        /// <summary>
        /// Returned when an identity value violates path-safety or security constraints.
        /// </summary>
        public static readonly DomainError SecurityViolation =
            Create(Codes.SecurityViolation, "Security violation.");

        /// <summary>
        /// Returned when a repository host segment is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankHost =
            Create(Codes.BlankHost, "Host cannot be blank.");

        /// <summary>
        /// Returned when a repository owner segment is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankOwner =
            Create(Codes.BlankOwner, "Owner cannot be blank.");

        /// <summary>
        /// Returned when a repository name segment is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankName =
            Create(Codes.BlankName, "Name cannot be blank.");

        /// <summary>
        /// Returned when a stream identifier is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankStream =
            Create(Codes.BlankStream, "Stream cannot be blank.");

        /// <summary>
        /// Returned when an object identifier is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankObjectId =
            Create(Codes.BlankObjectId, "Object ID cannot be blank.");

        /// <summary>
        /// Returned when a run identifier is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankRunId =
            Create(Codes.BlankRunId, "Run ID cannot be blank.");

        /// <summary>
        /// Returned when a source system identifier is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankSourceSystem =
            Create(Codes.BlankSourceSystem, "Source system cannot be blank.");

        /// <summary>
        /// Returned when a source system identifier does not satisfy canonical source-system rules.
        /// </summary>
        public static readonly DomainError InvalidSourceSystem =
            Create(Codes.InvalidSourceSystem, "Invalid source system.");

        /// <summary>
        /// Returned when a correlation identifier is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankCorrelationId =
            Create(Codes.BlankCorrelationId, "Correlation ID cannot be blank.");
    }
}