using Dx.Domain.Errors;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Defines errors related to manifest payloads and signatures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These errors are used when validating manifest payloads and signature tokens.
    /// </para>
    /// <para>
    /// Error codes are stable contracts. Messages remain safe to log and intentionally omit raw caller input.
    /// </para>
    /// </remarks>
    public static partial class Manifest
    {
        /// <summary>
        /// Defines stable error codes for manifest-related validation failures.
        /// </summary>
        public static class Codes
        {
            /// <summary>Returned when a manifest payload is <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankPayload = "provenance.manifest.blank_payload";

            /// <summary>Returned when a manifest signature token is <see langword="null"/>, empty, or whitespace.</summary>
            public const string BlankToken = "provenance.manifest.blank_token";
        }

        /// <summary>
        /// Returned when a manifest payload is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankPayload =
            Create(Codes.BlankPayload, "Manifest payload cannot be blank.");

        /// <summary>
        /// Returned when a manifest signature token is <see langword="null"/>, empty, or whitespace.
        /// </summary>
        public static readonly DomainError BlankToken =
            Create(Codes.BlankToken, "Manifest signature token cannot be blank.");
    }
}