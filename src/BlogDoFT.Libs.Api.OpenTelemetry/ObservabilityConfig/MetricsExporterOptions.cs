namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

public enum MetricsExporterOptions
{
    /// <summary>
    /// Disables metrics exporting.
    /// </summary>
    DoNotUse,

    /// <summary>
    /// Writes metrics to the console exporter.
    /// </summary>
    Console,

    /// <summary>
    /// Exposes metrics via the Prometheus endpoint.
    /// </summary>
    Prometheus,

    /// <summary>
    /// Sends metrics to an OTLP endpoint.
    /// </summary>
    Otlp,
}
