using Dx.Domain;
using Dx.Domain.Errors;
using Dx.Domain.Primitives;

using Provenance.Primitives;
using Provenance.Primitives.Errors;

using System.Collections.Generic;
using System.Linq;

using static Dx.Domain.Dx;

namespace Provenance.Protocol;

/// <summary>
/// Provides explicit conversion between domain envelopes and protocol DTOs.
/// </summary>
/// <remarks>
/// <para>
/// Domain-to-DTO conversion collapses validated domain types into stable wire values.
/// DTO-to-domain conversion validates raw wire values and aggregates independent validation failures where possible.
/// </para>
/// <para>
/// These mappings do not serialize raw Dx.Domain result or error objects into persisted DTOs. Conversion failures are
/// represented as Provenance protocol errors using stable diagnostic error codes.
/// </para>
/// </remarks>
public static class EnvelopeSurrogateExtensions
{
    /// <summary>
    /// Converts a domain envelope to its wire DTO representation.
    /// </summary>
    /// <param name="envelope">
    /// The validated domain envelope to convert. The envelope instance must not be null.
    /// </param>
    /// <returns>
    /// A successful result containing a <see cref="UniversalEnvelopeDto"/>, a failure result with
    /// <c>provenance.protocol.envelope_null</c> when <paramref name="envelope"/> is null, or a failure result with
    /// <c>provenance.protocol.envelope_invalid_dto</c> when the envelope contains invalid scalar, nullable,
    /// or protocol DTO state.
    /// </returns>
    public static Result<UniversalEnvelopeDto> ToDto(this UniversalEnvelope envelope)
    {
        if (envelope is null)
        {
            return Result.Failure<UniversalEnvelopeDto>(ProvenanceErrors.Protocol.EnvelopeNull);
        }

        List<DomainError> errors = ValidateEnvelopeForDto(envelope);

        if (errors.Count > 0)
        {
            return InvalidDtoFailure<UniversalEnvelopeDto>(errors);
        }

        VersionVector versionDomain = envelope.Version!;
        PayloadRef payloadDomain = envelope.Payload!;
        ProvenanceData provenanceDomain = envelope.Provenance!;

        VersionVectorDto version = new(
            SequenceNumber: versionDomain.SequenceNumber,
            Generation: versionDomain.Generation,
            PredecessorFingerprint: versionDomain.PredecessorFingerprint);

        PayloadRefDto payload = new(
            Cid: payloadDomain.Cid.Value,
            SizeBytes: payloadDomain.SizeBytes);

        ProvenanceDataDto provenance = new(
            CorrelationId: provenanceDomain.CorrelationId?.ToString(),
            TraceId: provenanceDomain.TraceId?.ToString(),
            CollectedBy: provenanceDomain.CollectedBy);

        EvidenceChainRefDto? evidence = envelope.Evidence is null
            ? null
            : new EvidenceChainRefDto(
                SourceCid: envelope.Evidence.SourceCid.Value,
                ByteOffset: envelope.Evidence.ByteOffset,
                LineIndex: envelope.Evidence.LineIndex);

        return Result.Success(new UniversalEnvelopeDto
        {
            SchemaVersion = envelope.SchemaVersion,
            CollectedAt = envelope.CollectedAt,
            SourceSystemId = envelope.SourceSystem.Value,
            Tenant = envelope.Tenant.Value,
            RepoHost = envelope.Repo.Host,
            RepoOwner = envelope.Repo.Owner,
            RepoName = envelope.Repo.Name,
            Stream = envelope.Stream.Value,
            ObjectId = envelope.ObjectId.Value,
            Version = version,
            Payload = payload,
            Provenance = provenance,
            Evidence = evidence,
        });
    }

    /// <summary>
    /// Converts a wire DTO representation into a validated domain envelope.
    /// </summary>
    /// <param name="dto">
    /// The DTO to validate and convert. The DTO instance must not be null.
    /// </param>
    /// <returns>
    /// A successful result containing a <see cref="UniversalEnvelope"/>, a failure result with
    /// <c>provenance.protocol.envelope_dto_null</c> when <paramref name="dto"/> is null, or a failure result with
    /// <c>provenance.protocol.envelope_invalid_dto</c>. Invalid DTO failures are derived from stable validation
    /// error codes.
    /// </returns>
    public static Result<UniversalEnvelope> ToDomain(this UniversalEnvelopeDto dto)
    {
        if (dto is null)
        {
            return Result.Failure<UniversalEnvelope>(ProvenanceErrors.Protocol.EnvelopeDtoNull);
        }

        List<DomainError> errors = new();

        if (dto.SchemaVersion < 1)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopeSchemaVersionInvalid);
        }

        Result<SourceSystemId> sourceSystemResult = SourceSystemId.Create(dto.SourceSystemId);
        AddIfFailure(sourceSystemResult, errors);

        Result<TenantId> tenantResult = TenantId.Create(dto.Tenant);
        AddIfFailure(tenantResult, errors);

        Result<RepoKey> repoResult = RepoKey.Create(dto.RepoHost, dto.RepoOwner, dto.RepoName);
        AddIfFailure(repoResult, errors);

        Result<StreamId> streamResult = StreamId.Create(dto.Stream);
        AddIfFailure(streamResult, errors);

        Result<ObjectId> objectResult = ObjectId.Create(dto.ObjectId);
        AddIfFailure(objectResult, errors);

        VersionVector? version = ConvertVersion(dto.Version, errors);
        PayloadRef? payload = ConvertPayload(dto.Payload, errors);
        ProvenanceData? provenance = ConvertProvenance(dto.Provenance, errors);
        EvidenceChainRef? evidence = ConvertEvidence(dto.Evidence, errors);

        if (errors.Count > 0)
        {
            return InvalidDtoFailure<UniversalEnvelope>(errors);
        }

        return UniversalEnvelope.Create(
            dto.SchemaVersion,
            dto.CollectedAt,
            sourceSystemResult.Value,
            tenantResult.Value,
            repoResult.Value,
            streamResult.Value,
            objectResult.Value,
            version!,
            payload!,
            provenance!,
            evidence);
    }

    private static List<DomainError> ValidateEnvelopeForDto(UniversalEnvelope envelope)
    {
        List<DomainError> errors = new();

        if (envelope.SchemaVersion < 1)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopeSchemaVersionInvalid);
        }

        if (envelope.Version is null)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopeVersionNull);
        }
        else
        {
            if (envelope.Version.SequenceNumber < 1)
            {
                errors.Add(ProvenanceErrors.Protocol.VersionSequenceInvalid);
            }

            if (envelope.Version.Generation < 1)
            {
                errors.Add(ProvenanceErrors.Protocol.VersionGenerationInvalid);
            }
        }

        if (envelope.Payload is null)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopePayloadNull);
        }
        else if (envelope.Payload.SizeBytes < 0)
        {
            errors.Add(ProvenanceErrors.Protocol.PayloadSizeNegative);
        }

        if (envelope.Provenance is null)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopeProvenanceNull);
        }
        else if (envelope.Provenance.CollectedBy is not null && string.IsNullOrWhiteSpace(envelope.Provenance.CollectedBy))
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopeProvenanceCollectedByInvalid);
        }

        if (envelope.Evidence is not null)
        {
            if (envelope.Evidence.ByteOffset < 0)
            {
                errors.Add(ProvenanceErrors.Protocol.EvidenceByteOffsetNegative);
            }

            if (envelope.Evidence.LineIndex < 0)
            {
                errors.Add(ProvenanceErrors.Protocol.EvidenceLineIndexNegative);
            }
        }

        return errors;
    }

    private static VersionVector? ConvertVersion(VersionVectorDto? dto, List<DomainError> errors)
    {
        if (dto is null)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopeVersionNull);
            return null;
        }

        int initialErrorCount = errors.Count;

        if (dto!.SequenceNumber < 1)
        {
            errors.Add(ProvenanceErrors.Protocol.VersionSequenceInvalid);
        }

        if (dto.Generation < 1)
        {
            errors.Add(ProvenanceErrors.Protocol.VersionGenerationInvalid);
        }

        if (errors.Count > initialErrorCount)
        {
            return null;
        }

        Result<VersionVector> result = VersionVector.Create(
            dto.SequenceNumber,
            dto.Generation,
            dto.PredecessorFingerprint);

        if (result.IsFailure)
        {
            errors.Add(result.Error);
            return null;
        }

        return result.Value;
    }

    private static PayloadRef? ConvertPayload(PayloadRefDto? dto, List<DomainError> errors)
    {
        if (dto is null)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopePayloadNull);
            return null;
        }

        int initialErrorCount = errors.Count;

        Result<ContentId> cidResult = ContentId.Parse(dto!.Cid);
        AddIfFailure(cidResult, errors);

        if (dto.SizeBytes < 0)
        {
            errors.Add(ProvenanceErrors.Protocol.PayloadSizeNegative);
        }

        if (errors.Count > initialErrorCount)
        {
            return null;
        }

        Result<PayloadRef> payloadResult = PayloadRef.Create(cidResult.Value, dto.SizeBytes);

        if (payloadResult.IsFailure)
        {
            errors.Add(payloadResult.Error);
            return null;
        }

        return payloadResult.Value;
    }

    private static ProvenanceData? ConvertProvenance(ProvenanceDataDto? dto, List<DomainError> errors)
    {
        if (dto is null)
        {
            errors.Add(ProvenanceErrors.Protocol.EnvelopeProvenanceNull);
            return null;
        }

        CorrelationId? correlationId = ParseCorrelationId(dto.CorrelationId, errors);
        TraceId? traceId = ParseTraceId(dto.TraceId, errors);

        Result<ProvenanceData> provenanceResult = ProvenanceData.Create(
            correlationId,
            traceId,
            dto.CollectedBy);

        if (provenanceResult.IsFailure)
        {
            errors.Add(provenanceResult.Error);
            return null;
        }

        return provenanceResult.Value;
    }

    private static EvidenceChainRef? ConvertEvidence(EvidenceChainRefDto? dto, List<DomainError> errors)
    {
        if (dto is null)
        {
            return null;
        }

        int initialErrorCount = errors.Count;

        Result<ContentId> sourceCidResult = ContentId.Parse(dto.SourceCid);
        AddIfFailure(sourceCidResult, errors);

        if (dto.ByteOffset < 0)
        {
            errors.Add(ProvenanceErrors.Protocol.EvidenceByteOffsetNegative);
        }

        if (dto.LineIndex < 0)
        {
            errors.Add(ProvenanceErrors.Protocol.EvidenceLineIndexNegative);
        }

        if (errors.Count > initialErrorCount)
        {
            return null;
        }

        Result<EvidenceChainRef> evidenceResult = EvidenceChainRef.Create(
            sourceCidResult.Value,
            dto.ByteOffset,
            dto.LineIndex);

        if (evidenceResult.IsFailure)
        {
            errors.Add(evidenceResult.Error);
            return null;
        }

        return evidenceResult.Value;
    }

    private static CorrelationId? ParseCorrelationId(string? value, List<DomainError> errors)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (CorrelationId.TryParse(value, null, out CorrelationId correlationId))
        {
            return correlationId;
        }

        errors.Add(ProvenanceErrors.Protocol.ProvenanceCorrelationInvalid(value));
        return null;
    }

    private static TraceId? ParseTraceId(string? value, List<DomainError> errors)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (TraceId.TryParse(value, null, out TraceId traceId))
        {
            return traceId;
        }

        errors.Add(ProvenanceErrors.Protocol.ProvenanceTraceInvalid(value));
        return null;
    }

    private static void AddIfFailure<T>(Result<T> result, List<DomainError> errors)
        where T : notnull
    {
        if (result.IsFailure)
        {
            errors.Add(result.Error);
        }
    }

    private static Result<T> InvalidDtoFailure<T>(IReadOnlyCollection<DomainError> errors)
        where T : notnull
    {
        string details = string.Join("; ", errors.Select(error => error.Code).Distinct());
        return Result.Failure<T>(ProvenanceErrors.Protocol.EnvelopeInvalidDto(details));
    }
}