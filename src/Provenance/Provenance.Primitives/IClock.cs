using NodaTime;

namespace Provenance.Primitives;

/// <summary>
/// Provides the current instant used by Provenance operations.
/// </summary>
/// <remarks>
/// <para>
/// Implementations must return UTC-based <see cref="Instant"/> values.
/// </para>
/// <para>
/// Production usage should use <see cref="ProvenanceClock"/>. Tests should use deterministic implementations
/// to avoid hidden current-time behavior.
/// </para>
/// </remarks>
public interface IClock
{
    /// <summary>
    /// Gets the current instant.
    /// </summary>
    /// <returns>
    /// The current UTC <see cref="Instant"/>.
    /// </returns>
    Instant GetCurrentInstant();
}