using Microsoft.Extensions.Configuration;

namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

public class OpenTelemetry
{
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

    public bool IncludeFormattedMessage { get; init; }

    public bool IncludeScopes { get; init; }

    public bool ParseStateValues { get; init; }

    public TracingExporterOptions UseTracingExporter { get; init; }

    public MetricsExporterOptions UseMetricsExporter { get; init; }

    public LogExporterOptions UseLogExporter { get; init; }

    public HistogramOptions HistogramAggregation { get; init; }

    public bool TracingActive => UseTracingExporter != TracingExporterOptions.DoNotUse;

    public bool MetricsActive => UseMetricsExporter != MetricsExporterOptions.DoNotUse;

    public bool LogsActive => UseLogExporter != LogExporterOptions.DotNotUse;
}
