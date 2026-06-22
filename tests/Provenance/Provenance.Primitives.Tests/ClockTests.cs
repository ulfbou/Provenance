using FluentAssertions;

using Provenance.Primitives;

using NodaTime;

using Xunit;

namespace Provenance.Primitives.Tests;

/// <summary>
/// Unit tests for the Provenance clock abstraction.
/// Validates that ProvenanceClock delegates to SystemClock and returns a valid Instant.
/// </summary>
[Trait("Category", "Unit")]
[Trait("Area", "Core")]
[Trait("Component", "Clock")]
[Trait("Version", "v0.1.0-alpha")]
public sealed class ClockTests
{
    /// <summary>ProvenanceClock should return a current, non-default Instant within a small tolerance.</summary>
    [Fact(DisplayName = "Provenance Clock Returns Current Instant")]
    public void ProvenanceClockReturnsCurrentInstant()
    {
        // Fully qualify to avoid NodaTime.IClock ambiguity
        Provenance.Primitives.IClock clock = new ProvenanceClock();

        var instant = clock.GetCurrentInstant();
        var now = SystemClock.Instance.GetCurrentInstant();

        // Basic sanity checks using FluentAssertions comparable API
        instant.Should().NotBe(default(Instant));
        instant.Should().BeGreaterThan(Instant.FromUnixTimeTicks(0));

        // Allow 2-second tolerance for test execution
        var diff = now - instant;
        diff.Should().BeLessThan(Duration.FromSeconds(2));
    }
}