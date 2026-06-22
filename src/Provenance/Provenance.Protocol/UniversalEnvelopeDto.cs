using Provenance.Primitives.Errors;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using NodaTime;
using NodaTime.Text;

namespace Provenance.Protocol;

public sealed record VersionVectorDto(
    [property: JsonPropertyName("sequence_number")] long SequenceNumber,
    [property: JsonPropertyName("generation")] int Generation,
    [property: JsonPropertyName("predecessor_fingerprint")] string? PredecessorFingerprint
);

public sealed record PayloadRefDto(
    [property: JsonPropertyName("cid")] string Cid,
    [property: JsonPropertyName("size_bytes")] long SizeBytes
);

public sealed record EvidenceChainRefDto(
    [property: JsonPropertyName("source_cid")] string SourceCid,
    [property: JsonPropertyName("byte_offset")] long ByteOffset,
    [property: JsonPropertyName("line_index")] int LineIndex
);

public sealed record ProvenanceDataDto(
    [property: JsonPropertyName("correlation_id")] string? CorrelationId,
    [property: JsonPropertyName("trace_id")] string? TraceId,
    [property: JsonPropertyName("collected_by")] string? CollectedBy
);

/// <summary>
/// Pure, string-collapsed serialization DTO for the wire protocol representation.
/// </summary>
public sealed record UniversalEnvelopeDto
{
    [JsonPropertyName("schema_version")]
    public int SchemaVersion { get; init; } = 2;

    [JsonPropertyName("collected_at")]
    public Instant CollectedAt { get; init; }

    [JsonPropertyName("source_system_id")]
    public string SourceSystemId { get; init; } = string.Empty;

    [JsonPropertyName("tenant")]
    public string Tenant { get; init; } = string.Empty;

    [JsonPropertyName("repo_host")]
    public string RepoHost { get; init; } = string.Empty;

    [JsonPropertyName("repo_owner")]
    public string RepoOwner { get; init; } = string.Empty;

    [JsonPropertyName("repo_name")]
    public string RepoName { get; init; } = string.Empty;

    [JsonPropertyName("stream")]
    public string Stream { get; init; } = string.Empty;

    [JsonPropertyName("object_id")]
    public string ObjectId { get; init; } = string.Empty;

    [JsonPropertyName("version")]
    public VersionVectorDto Version { get; init; } = null!;

    [JsonPropertyName("payload")]
    public PayloadRefDto Payload { get; init; } = null!;

    [JsonPropertyName("provenance")]
    public ProvenanceDataDto Provenance { get; init; } = null!;

    [JsonPropertyName("evidence")]
    public EvidenceChainRefDto? Evidence { get; init; }
}

/// <summary>
/// Source-generation compatible converter for NodaTime Instant values.
/// This makes the Instant serialization path explicit for ProtocolJsonContext.
/// </summary>
public sealed class InstantJsonConverter : JsonConverter<Instant>
{
    public override Instant Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected a JSON string token when reading a NodaTime Instant.");
        }

        string? value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("Cannot parse an empty JSON string as a NodaTime Instant.");
        }

        var parseResult = InstantPattern.ExtendedIso.Parse(value);

        if (!parseResult.Success)
        {
            throw new JsonException($"Invalid NodaTime Instant value: {value}");
        }

        return parseResult.Value;
    }

    public override void Write(
        Utf8JsonWriter writer,
        Instant value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(InstantPattern.ExtendedIso.Format(value));
    }
}

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    Converters = new[] { typeof(InstantJsonConverter) })]
[JsonSerializable(typeof(UniversalEnvelopeDto))]
[JsonSerializable(typeof(VersionVectorDto))]
[JsonSerializable(typeof(PayloadRefDto))]
[JsonSerializable(typeof(EvidenceChainRefDto))]
[JsonSerializable(typeof(ProvenanceDataDto))]
public partial class ProtocolJsonContext : JsonSerializerContext
{
}