using BlogDoFT.Libs.Api.OpenTelemetry.Extensions;
using BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlogDoFT.Libs.Api.OpenTelemetry.Tests.Extensions.OpenTelemetryExtensionTests;

public class AddOtelTests : OpenTelemetryExtensionBaseTests
{
    [Fact]
    public void Should_RegisterObservabilityOptions_When_ObservabilitySectionExists()
    {
        // Given
        var configData = BuildObservabilityConfiguration();
        var configuration = BuildConfiguration(configData);
        var services = CreateServices();

        // When
        services.AddOtel(configuration);

        // Then
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<Observability>>();
        options.Value.ShouldNotBeNull();
        options.Value.OpenTelemetry.UseMetricsExporter.ShouldBe(MetricsExporterOptions.Console);
        options.Value.OpenTelemetry.IncludeScopes.ShouldBeTrue();
    }

    [Fact]
    public void Should_ThrowInvalidOperation_When_ZipkinExporterIsEnabledWithoutOptions()
    {
        // Given
        var configData = BuildObservabilityConfiguration();
        configData["Observability:OpenTelemetry:UseTracingExporter"] = nameof(TracingExporterOptions.Zipkin);
        var configuration = BuildConfiguration(configData);
        var services = CreateServices();

        // When
        var act = () => services.AddOtel(configuration);

        // Then
        Should.Throw<InvalidOperationException>(act);
    }
}
