using Microsoft.Extensions.Configuration;

namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

/// <summary>
/// Configuration options for the ASP.NET Core instrumentation used by OpenTelemetry tracing.
/// </summary>
public class AspNetCoreInstrumentation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspNetCoreInstrumentation"/> class from the given
    /// configuration section.
    /// </summary>
    /// <param name="section">The configuration section containing the ASP.NET Core instrumentation settings.</param>
    public AspNetCoreInstrumentation(IConfigurationSection section)
    {
        RecordException = section.GetValue(nameof(RecordException), true);
    }

    /// <summary>
    /// Gets a value indicating whether unhandled exceptions should be recorded on the trace activity.
    /// Defaults to <see langword="true"/> when not configured.
    /// </summary>
    public bool RecordException { get; }
}
