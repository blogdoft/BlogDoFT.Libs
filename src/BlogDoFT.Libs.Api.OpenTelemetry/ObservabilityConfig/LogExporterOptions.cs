namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

public enum LogExporterOptions
{
    /// <summary>
    /// Disables log exporting.
    /// </summary>
    DotNotUse,

    /// <summary>
    /// Sends log entries to the console.
    /// </summary>
    Console,

    /// <summary>
    /// Sends log entries to an OTLP endpoint.
    /// </summary>
    Otlp,
}
