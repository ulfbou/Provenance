using Dx.Domain.Errors;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Provides stable domain errors for clock validation, including future instants, unspecified DateTime kinds, and system clock skew.
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
    public static class Clock
    {
        /// <summary>
        /// Defines stable error codes for clock-related errors.
        /// </summary>
        public static class Codes
        {
            /// <summary>
            /// Returned when an instant is in the future relative to the active Provenance clock.
            /// </summary>
            public const string FutureInstant = "provenance.clock.future_instant";

            /// <summary>
            /// Returned when a <see cref="System.DateTime"/> value has <see cref="System.DateTimeKind.Unspecified"/>.
            /// </summary>
            public const string UnspecifiedKind = "provenance.clock.unspecified_kind";

            /// <summary>
            /// Returned when the system clock produces a non-monotonic or otherwise invalid temporal observation.
            /// </summary>
            public const string SystemClockSkew = "provenance.clock.system_clock_skew";
        }

        /// <summary>
        /// Returned when an instant is in the future relative to the active Provenance clock.
        /// </summary>
        public static readonly DomainError FutureInstant =
            Create(Codes.FutureInstant, "The provided instant is in the future relative to the active provenance clock.");

        /// <summary>
        /// Returned when a <see cref="System.DateTime"/> value has <see cref="System.DateTimeKind.Unspecified"/>.
        /// </summary>
        public static readonly DomainError UnspecifiedKind =
            Create(Codes.UnspecifiedKind, "The provided DateTime value has DateTimeKind.Unspecified and cannot be converted deterministically.");

        /// <summary>
        /// Returned when the system clock produces a non-monotonic or otherwise invalid temporal observation.
        /// </summary>
        public static readonly DomainError SystemClockSkew =
            Create(Codes.SystemClockSkew, "The system clock produced a non-monotonic or otherwise invalid temporal observation.");
    }
}