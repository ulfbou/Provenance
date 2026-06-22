using Dx.Domain.Errors;

using System.Collections.Generic;

namespace Provenance.Primitives.Errors;

public static partial class ProvenanceErrors
{
    /// <summary>
    /// Defines errors related to protocol models and DTO validation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These errors are used when converting between protocol DTOs and Provenance domain types.
    /// </para>
    /// <para>
    /// Error codes are stable contracts. Messages remain safe to log and intentionally omit raw caller input.
    /// </para>
    /// </remarks>
    public static partial class Protocol
    {
        /// <summary>
        /// Defines stable error codes for protocol-related validation failures.
        /// </summary>
        public static class Codes
        {
            /// <summary>Returned when a version sequence value is less than one.</summary>
            public const string VersionSequenceInvalid = "provenance.protocol.version_sequence_invalid";

            /// <summary>Returned when a version generation value is less than one.</summary>
            public const string VersionGenerationInvalid = "provenance.protocol.version_generation_invalid";

            /// <summary>Returned when a payload size is negative.</summary>
            public const string PayloadSizeNegative = "provenance.protocol.payload_size_negative";

            /// <summary>Returned when an evidence byte offset is negative.</summary>
            public const string EvidenceByteOffsetNegative = "provenance.protocol.evidence_byte_offset_negative";

            /// <summary>Returned when an evidence line index is negative.</summary>
            public const string EvidenceLineIndexNegative = "provenance.protocol.evidence_line_index_negative";

            /// <summary>Returned when an envelope schema version is less than one.</summary>
            public const string EnvelopeSchemaVersionInvalid = "provenance.protocol.envelope_schema_version_invalid";

            /// <summary>Returned when an envelope version component is <see langword="null"/>.</summary>
            public const string EnvelopeVersionNull = "provenance.protocol.envelope_version_null";

            /// <summary>Returned when an envelope payload component is <see langword="null"/>.</summary>
            public const string EnvelopePayloadNull = "provenance.protocol.envelope_payload_null";

            /// <summary>Returned when an envelope provenance component is <see langword="null"/>.</summary>
            public const string EnvelopeProvenanceNull = "provenance.protocol.envelope_provenance_null";

            /// <summary>Returned when a provenance correlation identifier is invalid.</summary>
            public const string ProvenanceCorrelationInvalid = "provenance.protocol.provenance_correlation_invalid";

            /// <summary>Returned when a provenance trace identifier is invalid.</summary>
            public const string ProvenanceTraceInvalid = "provenance.protocol.provenance_trace_invalid";

            /// <summary>Returned when an envelope DTO fails structural validation.</summary>
            public const string EnvelopeInvalidDto = "provenance.protocol.envelope_invalid_dto";

            /// <summary>Returned when a domain envelope instance is <see langword="null"/>.</summary>
            public const string EnvelopeNull = "provenance.protocol.envelope_null";

            /// <summary>Returned when a DTO envelope instance is <see langword="null"/>.</summary>
            public const string EnvelopeDtoNull = "provenance.protocol.envelope_dto_null";

            /// <summary>Returned when an envelope provenance collected by component is invalid.</summary>
            public const string EnvelopeProvenanceCollectedByInvalid = "provenance.protocol.envelope_provenance_collected_by_invalid";
        }

        /// <summary>
        /// Returned when a domain envelope conversion receives a null envelope instance.
        /// </summary>
        public static readonly DomainError EnvelopeNull =
            Create(Codes.EnvelopeNull, "Envelope must not be null.");

        /// <summary>
        /// Returned when DTO-to-domain conversion receives a null DTO instance.
        /// </summary>
        public static readonly DomainError EnvelopeDtoNull =
            Create(Codes.EnvelopeDtoNull, "Envelope DTO must not be null.");

        /// <summary>
        /// Returned when a version sequence value is less than one.
        /// </summary>
        public static readonly DomainError VersionSequenceInvalid =
            Create(Codes.VersionSequenceInvalid, "Sequence number must be greater than or equal to 1.");

        /// <summary>
        /// Returned when a version generation value is less than one.
        /// </summary>
        public static readonly DomainError VersionGenerationInvalid =
            Create(Codes.VersionGenerationInvalid, "Generation must be greater than or equal to 1.");

        /// <summary>
        /// Returned when a protocol payload size is negative.
        /// </summary>
        public static readonly DomainError PayloadSizeNegative =
            Create(Codes.PayloadSizeNegative, "Size in bytes must be non-negative.");

        /// <summary>
        /// Returned when an evidence byte offset is negative.
        /// </summary>
        public static readonly DomainError EvidenceByteOffsetNegative =
            Create(Codes.EvidenceByteOffsetNegative, "Byte offset must be non-negative.");

        /// <summary>
        /// Returned when an evidence line index is negative.
        /// </summary>
        public static readonly DomainError EvidenceLineIndexNegative =
            Create(Codes.EvidenceLineIndexNegative, "Line index must be non-negative.");

        /// <summary>
        /// Returned when an envelope schema version is less than one.
        /// </summary>
        public static readonly DomainError EnvelopeSchemaVersionInvalid =
            Create(Codes.EnvelopeSchemaVersionInvalid, "Schema version must be greater than or equal to 1.");

        /// <summary>
        /// Returned when an envelope version component is null.
        /// </summary>
        public static readonly DomainError EnvelopeVersionNull =
            Create(Codes.EnvelopeVersionNull, "Version must not be null.");

        /// <summary>
        /// Returned when an envelope payload component is null.
        /// </summary>
        public static readonly DomainError EnvelopePayloadNull =
            Create(Codes.EnvelopePayloadNull, "Payload must not be null.");

        /// <summary>
        /// Returned when an envelope provenance component is null.
        /// </summary>
        public static readonly DomainError EnvelopeProvenanceNull =
            Create(Codes.EnvelopeProvenanceNull, "Provenance must not be null.");

        /// <summary>
        /// Returned when an envelope provenance collected by component is invalid.
        /// </summary>
        public static readonly DomainError EnvelopeProvenanceCollectedByInvalid =
            Create(ProvenanceErrors.Protocol.Codes.EnvelopeProvenanceCollectedByInvalid, "Collected by must not be null or whitespace.");

        /// <summary>
        /// Creates an error for an invalid provenance correlation identifier.
        /// </summary>
        /// <param name="value">
        /// The invalid correlation identifier value. The value is intentionally omitted from the public message.
        /// </param>
        /// <returns>
        /// A <see cref="DomainError"/> with code <c>provenance.protocol.provenance_correlation_invalid</c>.
        /// </returns>
        public static DomainError ProvenanceCorrelationInvalid(string value) =>
            Create(
                Codes.ProvenanceCorrelationInvalid,
                "Invalid correlation identifier.",
                KeyValuePair.Create<string, object>(
                    "ValidationDetails",
                    string.IsNullOrWhiteSpace(value) ? string.Empty : "Invalid correlation identifier."));

        /// <summary>
        /// Creates an error for an invalid provenance trace identifier.
        /// </summary>
        /// <param name="value">
        /// The invalid trace identifier value. The value is intentionally omitted from the public message.
        /// </param>
        /// <returns>
        /// A <see cref="DomainError"/> with code <c>provenance.protocol.provenance_trace_invalid</c>.
        /// </returns>
        public static DomainError ProvenanceTraceInvalid(string value) =>
            Create(
                Codes.ProvenanceTraceInvalid,
                "Invalid trace identifier.",
                KeyValuePair.Create<string, object>(
                    "ValidationDetails",
                    string.IsNullOrWhiteSpace(value) ? string.Empty : "Invalid trace identifier."));

        /// <summary>
        /// Creates an error for an invalid universal envelope DTO.
        /// </summary>
        /// <param name="details">
        /// Caller-provided validation detail. The value is intentionally omitted from the public message.
        /// </param>
        /// <returns>
        /// A <see cref="DomainError"/> with code <c>provenance.protocol.envelope_invalid_dto</c>.
        /// </returns>
#if false
        public static DomainError EnvelopeInvalidDto(string details) =>
            Create(
                Codes.EnvelopeInvalidDto,
                "Failed to convert UniversalEnvelopeDto to UniversalEnvelope.",
                KeyValuePair.Create<string, object>(
                    "ValidationDetails",
                    string.IsNullOrWhiteSpace(details) ? string.Empty : "Structural validation failed."));
#else
        public static DomainError EnvelopeInvalidDto(string details) =>
            Create(
                Codes.EnvelopeInvalidDto,
                $"Failed to convert UniversalEnvelopeDto to UniversalEnvelope due to the following errors: {details}",
                KeyValuePair.Create<string, object>(
                    "ValidationDetails",
                    details ?? string.Empty));
#endif
    }
}