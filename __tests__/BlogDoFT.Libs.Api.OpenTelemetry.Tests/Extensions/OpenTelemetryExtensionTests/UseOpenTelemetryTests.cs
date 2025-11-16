using BlogDoFT.Libs.Api.OpenTelemetry.Extensions;
using BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.Api.OpenTelemetry.Tests.Extensions.OpenTelemetryExtensionTests;

public class UseOpenTelemetryTests : OpenTelemetryExtensionBaseTests
{
    [Fact]
    public void Should_MapPrometheusEndpoint_When_PrometheusExporterIsEnabled()
    {
        // Given
        var configData = BuildObservabilityConfiguration();
        configData["Observability:OpenTelemetry:UseMetricsExporter"] = nameof(MetricsExporterOptions.Prometheus);
        configData["Observability:PrometheusAspNetCoreOptions:ScrapeEndpointPath"] = "/metrics-test";
        var configuration = BuildConfiguration(configData);
        var services = CreateServices();
        services.AddOtel(configuration);
        using var provider = services.BuildServiceProvider();
        var builder = new TestApplicationBuilder(provider);

        // When
        builder.UseOpenTelemetry();

        // Then
        builder.DataSources.SelectMany(ds => ds.Endpoints).ShouldNotBeEmpty();
    }

    [Fact]
    public void Should_NotMapPrometheusEndpoint_When_PrometheusExporterIsDisabled()
    {
        // Given
        var configData = BuildObservabilityConfiguration();
        configData["Observability:OpenTelemetry:UseMetricsExporter"] = nameof(MetricsExporterOptions.Console);
        var configuration = BuildConfiguration(configData);
        var services = CreateServices();
        services.AddOtel(configuration);
        using var provider = services.BuildServiceProvider();
        var builder = new TestApplicationBuilder(provider);

        // When
        builder.UseOpenTelemetry();

        // Then
        builder.DataSources.SelectMany(ds => ds.Endpoints).ShouldBeEmpty();
    }

    private sealed class TestApplicationBuilder : IApplicationBuilder, IEndpointRouteBuilder
    {
        private readonly ApplicationBuilder _inner;

        public TestApplicationBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _inner = new ApplicationBuilder(serviceProvider);
            DataSources = new List<EndpointDataSource>();
        }

        public IServiceProvider ServiceProvider { get; }

        public ICollection<EndpointDataSource> DataSources { get; }

        public IFeatureCollection ServerFeatures => _inner.ServerFeatures;

        public IDictionary<string, object?> Properties => _inner.Properties;

        public IServiceProvider ApplicationServices
        {
            get => _inner.ApplicationServices;
            set => _inner.ApplicationServices = value;
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            _inner.Use(middleware);
            return this;
        }

        public IApplicationBuilder New() => _inner.New();

        public RequestDelegate Build() => _inner.Build();

        public IApplicationBuilder CreateApplicationBuilder() => new ApplicationBuilder(ServiceProvider);
    }
}
