using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;

namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

/// <summary>
/// Root configuration for OpenTelemetry observability, read from the <c>Observability</c> section of the
/// application configuration.
/// </summary>
public class Observability
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Observability"/> class, reading its nested sections
    /// (OpenTelemetry, Zipkin, OTLP, ASP.NET Core instrumentation and Prometheus) from <paramref name="configuration"/>.
    /// </summary>
    /// <param name="configuration">The application configuration containing the <c>Observability</c> section.</param>
    public Observability(IConfiguration configuration)
    {
        var openTelemetrySection = configuration
            .GetSection($"{nameof(Observability)}:{nameof(OpenTelemetry)}");
        OpenTelemetry = new OpenTelemetry(openTelemetrySection);

        ZipkinExporterOptions = configuration
            .GetSection($"{nameof(Observability)}:{nameof(ZipkinExporterOptions)}")
            .Get<ZipkinExporterOptions>();

        OtlpExporterOptions = configuration
            .GetSection($"{nameof(Observability)}:{nameof(OtlpExporterOptions)}")
            .Get<OtlpExporterOptions>();

        AspNetCoreInstrumentation = new AspNetCoreInstrumentation(configuration
            .GetSection($"{nameof(Observability)}:{nameof(AspNetCoreInstrumentation)}"));

        PrometheusAspNetCoreOptions = configuration
            .GetSection($"{nameof(Observability)}:{nameof(PrometheusAspNetCoreOptions)}")?
            .Get<PrometheusAspNetCoreOptions>();
    }

    /// <summary>
    /// Gets or sets the general OpenTelemetry settings (exporters, aggregation, active signals).
    /// </summary>
    public OpenTelemetry OpenTelemetry { get; set; }

    /// <summary>
    /// Gets or sets the Zipkin exporter options, or <see langword="null"/> when not configured.
    /// </summary>
    public ZipkinExporterOptions? ZipkinExporterOptions { get; set; }

    /// <summary>
    /// Gets or sets the OTLP exporter options, or <see langword="null"/> when not configured.
    /// </summary>
    public OtlpExporterOptions? OtlpExporterOptions { get; set; }

    /// <summary>
    /// Gets or sets the ASP.NET Core instrumentation options.
    /// </summary>
    public AspNetCoreInstrumentation AspNetCoreInstrumentation { get; set; }

    /// <summary>
    /// Gets or sets the Prometheus ASP.NET Core exporter options, or <see langword="null"/> when not configured.
    /// </summary>
    public PrometheusAspNetCoreOptions? PrometheusAspNetCoreOptions { get; set; }
}
