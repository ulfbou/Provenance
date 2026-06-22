using Dx.Domain.Errors;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;

namespace Provenance.Primitives.Errors;

/// <summary>
/// Defines the central error catalog for Provenance.Primitives domain validation and kernel operations.
/// </summary>
/// <remarks>
/// <para>
/// Error codes are stable public contracts using the form
/// <c>provenance.&lt;area&gt;.&lt;specific_failure&gt;</c>.
/// </para>
/// <para>
/// Consumers must rely on <see cref="DomainError.Code"/> for programmatic behavior. Error messages are
/// human-readable diagnostics and evolve without breaking compatibility.
/// </para>
/// <para>
/// Error messages must remain safe to log and must not contain secrets, raw caller input, filesystem paths,
/// exception messages, serialized payload fragments, or other sensitive data.
/// </para>
/// </remarks>
public static partial class ProvenanceErrors
{
    private static DomainError Create(string code, string message) =>
            DomainError.Create(code, message, ImmutableArray<KeyValuePair<string, object>>.Empty);

    private static DomainError Create(string code, string message, params KeyValuePair<string, object>[] properties) =>
        DomainError.Create(code, message, ImmutableArray.Create(properties));

    private static string FormatSeconds(TimeSpan timeout) =>
        timeout.TotalSeconds.ToString("F1", CultureInfo.InvariantCulture);
}