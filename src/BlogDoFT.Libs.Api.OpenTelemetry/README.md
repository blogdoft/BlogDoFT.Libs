# BlogDoFT.Libs.Api.OpenTelemetry

Helpers to integrate OpenTelemetry tracing, metrics and logs into ASP.NET Core APIs.

This library provides opinionated wiring and small helpers to capture common HTTP server telemetry, plus configuration mapping from `IConfiguration` so you can control exporters and instrumentations from `appsettings.json`.

Features
- Easy registration of OpenTelemetry tracing, metrics and logs via a single `AddOtel` extension method
- Configuration object model that maps to `IConfiguration` (see sample `appsettings.json` below)
- Automatic registration of exporters (Console / Zipkin / OTLP / Prometheus) based on configuration

Quick start

1. Reference the package in your Web API project.
2. In `Program.cs` or `Startup.cs`, call the extension methods:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOtel(builder.Configuration);

var app = builder.Build();
app.UseOpenTelemetry();

app.Run();
```

Configuration (appsettings.json)

The library reads an `Observability` section from `IConfiguration`. Minimal example enabling console tracing and metrics:

```json
{
	"ApplicationName": "MyWebApi",
	"Observability": {
		"OpenTelemetry": {
			"IncludeFormattedMessage": true,
			"IncludeScopes": true,
			"ParseStateValues": true,
			"UseTracingExporter": "Console",
			"UseMetricsExporter": "Console",
			"UseLogExporter": "Console",
			"HistogramAggregation": "ExplicitBounds"
		}
	}
}
```

Enabling Zipkin exporter:

```json
{
	"Observability": {
		"OpenTelemetry": {
			"UseTracingExporter": "Zipkin"
		},
		"ZipkinExporterOptions": {
			"Endpoint": "http://localhost:9411/api/v2/spans"
		}
	}
}
```

Enabling OTLP exporter (gRPC):

```json
{
	"Observability": {
		"OpenTelemetry": {
			"UseTracingExporter": "Otlp",
			"UseMetricsExporter": "Otlp"
		},
		"OtlpExporterOptions": {
			"Endpoint": "http://otel-collector:4317",
			"Protocol": "Grpc"
		}
	}
}
```

Prometheus metrics

To enable Prometheus scraping endpoint set the metrics exporter to `Prometheus` and (optionally) configure `PrometheusAspNetCoreOptions`.

Notes
- The library maps configuration into the `Observability` object model. See the `Observability` and nested classes for all available fields and defaults.
- When using OTLP, the default endpoint is `http://localhost:4317` and default protocol is gRPC.
