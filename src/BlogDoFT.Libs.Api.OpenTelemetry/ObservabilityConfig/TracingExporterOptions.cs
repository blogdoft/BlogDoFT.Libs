namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

/// <summary>
/// Defines the destination used to export trace data.
/// </summary>
public enum TracingExporterOptions
{
    /// <summary>
    /// Disables tracing exporter configuration.
    /// </summary>
    DoNotUse,

    /// <summary>
    /// Sends traces to the console exporter.
    /// </summary>
    Console,

    /// <summary>
    /// Sends traces to an OTLP endpoint.
    /// </summary>
    Otlp,

    /// <summary>
    /// Sends traces to a Zipkin endpoint.
    /// </summary>
    Zipkin,
}
