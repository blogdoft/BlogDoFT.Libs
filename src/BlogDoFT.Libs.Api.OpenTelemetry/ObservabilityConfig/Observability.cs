using Microsoft.Extensions.Configuration;
using OpenTelemetry.Exporter;

namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

public class Observability
{
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

    public OpenTelemetry OpenTelemetry { get; set; }

    public ZipkinExporterOptions? ZipkinExporterOptions { get; set; }

    public OtlpExporterOptions? OtlpExporterOptions { get; set; }

    public AspNetCoreInstrumentation AspNetCoreInstrumentation { get; set; }

    public PrometheusAspNetCoreOptions? PrometheusAspNetCoreOptions { get; set; }
}
