using NodaTime;

namespace Provenance.Primitives;

/// <summary>
/// Provides the default production <see cref="IClock"/> implementation backed by the system clock.
/// </summary>
/// <remarks>
/// <para>
/// This implementation delegates to <see cref="SystemClock.Instance"/>.
/// </para>
/// <para>
/// Because this implementation reads the current wall-clock time, deterministic tests should use a fixed
/// or fake <see cref="IClock"/> implementation instead.
/// </para>
/// </remarks>
public sealed class ProvenanceClock : IClock
{
    /// <summary>
    /// Gets the current system instant.
    /// </summary>
    /// <returns>
    /// The current UTC <see cref="Instant"/> as provided by <see cref="SystemClock"/>.
    /// </returns>
    public Instant GetCurrentInstant() => SystemClock.Instance.GetCurrentInstant();
}