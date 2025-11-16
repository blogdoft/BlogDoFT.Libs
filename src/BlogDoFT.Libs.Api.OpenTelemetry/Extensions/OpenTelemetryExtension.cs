using BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;

namespace BlogDoFT.Libs.Api.OpenTelemetry.Extensions;

public static class OpenTelemetryExtension
{
    public static IServiceCollection AddOtel(this IServiceCollection services, IConfiguration configuration)
    {
        var hasObservability = configuration.GetSection(nameof(Observability)).Exists();

        if (!hasObservability)
        {
            return services;
        }

        var observability = new Observability(configuration);
        services.AddSingleton(Options.Create(observability));

        return services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: configuration["ApplicationName"] ?? "WebApi"))
            .AddMetrics(observability, services)
            .AddTracing(observability, services)
            .AddLogs(observability, services)
            .Services;
    }

    public static IApplicationBuilder UseOpenTelemetry(this IApplicationBuilder app)
    {
        var observability = app.ApplicationServices.GetRequiredService<IOptions<Observability>>().Value;

        if (observability.OpenTelemetry.UseMetricsExporter == MetricsExporterOptions.Prometheus)
        {
            (app as IEndpointRouteBuilder)?.MapPrometheusScrapingEndpoint();
        }

        return app;
    }

    private static IOpenTelemetryBuilder AddMetrics(this IOpenTelemetryBuilder otel, Observability observability, IServiceCollection services)
    {
        if (!observability.OpenTelemetry.MetricsActive)
        {
            return otel;
        }

        return otel.WithMetrics(metrics =>
            {
                metrics
                    .SetExemplarFilter(ExemplarFilterType.TraceBased)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http")
                    .AddMeter("System.Net.NameResolution");

                switch (observability.OpenTelemetry?.HistogramAggregation)
                {
                    case HistogramOptions.Exponential:
                        metrics.AddView(instrument =>
                        {
                            return instrument.GetType().GetGenericTypeDefinition() == typeof(Histogram<>)
                                ? new Base2ExponentialBucketHistogramConfiguration()
                                : null;
                        });
                        break;
                    default:
                        // Explicit bounds histogram is the default.
                        // No additional configuration necessary.
                        break;
                }

                switch (observability.OpenTelemetry?.UseMetricsExporter)
                {
                    case MetricsExporterOptions.Prometheus:
                        if (observability.PrometheusAspNetCoreOptions is not null)
                        {
                            services.AddSingleton(Options.Create(observability.PrometheusAspNetCoreOptions));
                        }

                        metrics.AddPrometheusExporter();
                        break;
                    case MetricsExporterOptions.Otlp:
                        if (observability.OtlpExporterOptions is not null)
                        {
                            services.AddSingleton(Options.Create(observability.OtlpExporterOptions));
                        }

                        metrics.AddOtlpExporter();
                        break;
                    default:
                        metrics.AddConsoleExporter();
                        break;
                }
            });
    }

    private static IOpenTelemetryBuilder AddTracing(this IOpenTelemetryBuilder otel, Observability observability, IServiceCollection services)
    {
        if (!observability.OpenTelemetry.TracingActive)
        {
            return otel;
        }

        return otel
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();

                services.Configure<AspNetCoreTraceInstrumentationOptions>(opt =>
                opt.RecordException = observability.AspNetCoreInstrumentation?.RecordException ?? true);

                switch (observability.OpenTelemetry?.UseTracingExporter)
                {
                    case TracingExporterOptions.Zipkin:
                        if (observability.ZipkinExporterOptions is null)
                        {
                            throw new InvalidOperationException("ZipkinExporterOptions cannot be null when using Zipkin exporter.");
                        }

                        tracing.ConfigureServices(services =>
                            services.AddSingleton(Options.Create(observability.ZipkinExporterOptions!)));

                        tracing.AddZipkinExporter();
                        break;

                    case TracingExporterOptions.Otlp:
                        tracing.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Protocol = observability.OtlpExporterOptions?.Protocol ?? OtlpExportProtocol.Grpc;
                            otlpOptions.Endpoint = observability.OtlpExporterOptions?.Endpoint ?? new Uri("http://localhost:4317");
                        });
                        break;

                    default:
                        tracing.AddConsoleExporter();
                        break;
                }
            });
    }

    private static IOpenTelemetryBuilder AddLogs(this IOpenTelemetryBuilder otel, Observability observability, IServiceCollection services)
    {
        if (!observability.OpenTelemetry.LogsActive)
        {
            return otel;
        }

        return otel
            .WithLogging(builder =>
            {
                switch (observability.OpenTelemetry?.UseLogExporter)
                {
                    case LogExporterOptions.Otlp:

                        if (observability.OtlpExporterOptions is not null)
                        {
                            services.AddSingleton(Options.Create(observability.OtlpExporterOptions));
                        }

                        builder.AddOtlpExporter();
                        break;
                    default:
                        builder.AddConsoleExporter();
                        break;
                }
            });
    }
}
