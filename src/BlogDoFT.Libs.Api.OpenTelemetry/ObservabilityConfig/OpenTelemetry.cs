using Microsoft.Extensions.Configuration;

namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

/// <summary>
/// General OpenTelemetry settings controlling which signals (tracing, metrics, logs) are active and which
/// exporters and aggregation strategy they use.
/// </summary>
public class OpenTelemetry
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTelemetry"/> class from the given configuration section.
    /// </summary>
    /// <param name="section">The configuration section containing the OpenTelemetry settings.</param>
    public OpenTelemetry(IConfigurationSection section)
    {
        IncludeFormattedMessage = section.GetValue(nameof(IncludeFormattedMessage), true);
        IncludeScopes = section.GetValue<bool>(nameof(IncludeScopes), true);
        ParseStateValues = section.GetValue<bool>(nameof(ParseStateValues), true);
        UseTracingExporter = section.GetValue<TracingExporterOptions>(nameof(UseTracingExporter));
        UseMetricsExporter = section.GetValue<MetricsExporterOptions>(nameof(UseMetricsExporter));
        UseLogExporter = section.GetValue<LogExporterOptions>(nameof(UseLogExporter));
        HistogramAggregation = section.GetValue<HistogramOptions>(nameof(HistogramAggregation));
    }

    /// <summary>
    /// Gets a value indicating whether the formatted log message should be included in log records.
    /// Defaults to <see langword="true"/> when not configured.
    /// </summary>
    public bool IncludeFormattedMessage { get; init; }

    /// <summary>
    /// Gets a value indicating whether logging scopes should be included in log records.
    /// Defaults to <see langword="true"/> when not configured.
    /// </summary>
    public bool IncludeScopes { get; init; }

    /// <summary>
    /// Gets a value indicating whether structured log state values should be parsed.
    /// Defaults to <see langword="true"/> when not configured.
    /// </summary>
    public bool ParseStateValues { get; init; }

    /// <summary>
    /// Gets the exporter used for tracing.
    /// </summary>
    public TracingExporterOptions UseTracingExporter { get; init; }

    /// <summary>
    /// Gets the exporter used for metrics.
    /// </summary>
    public MetricsExporterOptions UseMetricsExporter { get; init; }

    /// <summary>
    /// Gets the exporter used for logs.
    /// </summary>
    public LogExporterOptions UseLogExporter { get; init; }

    /// <summary>
    /// Gets the bucketing strategy used for histogram metrics.
    /// </summary>
    public HistogramOptions HistogramAggregation { get; init; }

    /// <summary>
    /// Gets a value indicating whether tracing is active, i.e. <see cref="UseTracingExporter"/> is not
    /// <see cref="TracingExporterOptions.DoNotUse"/>.
    /// </summary>
    public bool TracingActive => UseTracingExporter != TracingExporterOptions.DoNotUse;

    /// <summary>
    /// Gets a value indicating whether metrics collection is active, i.e. <see cref="UseMetricsExporter"/> is not
    /// <see cref="MetricsExporterOptions.DoNotUse"/>.
    /// </summary>
    public bool MetricsActive => UseMetricsExporter != MetricsExporterOptions.DoNotUse;

    /// <summary>
    /// Gets a value indicating whether log exporting is active, i.e. <see cref="UseLogExporter"/> is not
    /// <see cref="LogExporterOptions.DotNotUse"/>.
    /// </summary>
    public bool LogsActive => UseLogExporter != LogExporterOptions.DotNotUse;
}
