using FluentAssertions;

using System.Text.Json;

using NodaTime;

namespace Provenance.Protocol.Tests;

[Trait("Category", "Unit")]
[Trait("Area", "Protocol")]
[Trait("Component", "Serialization")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class InstantJsonConverterTests
{
    static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new InstantJsonConverter());
        return options;
    }

    [Theory(DisplayName = "Serialize should write extended ISO instant string")]
    [InlineData(2024, 1, 2, 3, 4, 5, 0, "2024-01-02T03:04:05Z")]
    [InlineData(2024, 1, 2, 3, 4, 5, 123456789, "2024-01-02T03:04:05.123456789Z")]
    public void Serialize_ShouldWriteExtendedIsoInstantString(
        int year,
        int month,
        int day,
        int hour,
        int minute,
        int second,
        int nanosecond,
        string expected)
    {
        var instant = Instant.FromUtc(year, month, day, hour, minute, second).PlusNanoseconds(nanosecond);

        var json = JsonSerializer.Serialize(instant, CreateOptions());

        json.Should().Be($"\"{expected}\"");
    }

    [Fact(DisplayName = "Deserialize should read extended ISO instant string")]
    public void Deserialize_ShouldReadExtendedIsoInstantString()
    {
        const string json = "\"2024-01-02T03:04:05.123456789Z\"";

        var instant = JsonSerializer.Deserialize<Instant>(json, CreateOptions());

        instant.Should().Be(Instant.FromUtc(2024, 1, 2, 3, 4, 5).PlusNanoseconds(123456789));
    }

    [Theory(DisplayName = "Deserialize should throw JsonException when JSON token is invalid")]
    [InlineData("123")]
    [InlineData("true")]
    [InlineData("{}")]
    public void Deserialize_ShouldThrowJsonException_WhenJsonTokenIsInvalid(string json)
    {
        var act = () => JsonSerializer.Deserialize<Instant>(json, CreateOptions());

        act.Should().Throw<JsonException>()
            .WithMessage("Expected a JSON string token when reading a NodaTime Instant.");
    }

    [Theory(DisplayName = "Deserialize should throw JsonException when instant string is blank")]
    [InlineData("\"\"")]
    [InlineData("\" \"")]
    public void Deserialize_ShouldThrowJsonException_WhenInstantStringIsBlank(string json)
    {
        var act = () => JsonSerializer.Deserialize<Instant>(json, CreateOptions());

        act.Should().Throw<JsonException>()
            .WithMessage("Cannot parse an empty JSON string as a NodaTime Instant.");
    }

    [Fact(DisplayName = "Deserialize should throw JsonException when instant string is malformed")]
    public void Deserialize_ShouldThrowJsonException_WhenInstantStringIsMalformed()
    {
        var act = () => JsonSerializer.Deserialize<Instant>("\"not-an-instant\"", CreateOptions());

        act.Should().Throw<JsonException>()
            .WithMessage("Invalid NodaTime Instant value: not-an-instant");
    }
}