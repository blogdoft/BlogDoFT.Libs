using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.Api.OpenTelemetry.Tests.Extensions.OpenTelemetryExtensionTests;

public abstract class OpenTelemetryExtensionBaseTests
{
    protected OpenTelemetryExtensionBaseTests()
    {
        Faker = Fixture.Get();
    }

    protected Faker Faker { get; }

    protected static IServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRouting();
        return services;
    }

    protected static IConfiguration BuildConfiguration(IDictionary<string, string?>? overrides = null)
    {
        var data = overrides ?? new Dictionary<string, string?>();
        return new ConfigurationBuilder()
            .AddInMemoryCollection(data)
            .Build();
    }

    protected static IDictionary<string, string?> BuildObservabilityConfiguration()
    {
        return new Dictionary<string, string?>
        {
            ["Observability:OpenTelemetry:IncludeFormattedMessage"] = "true",
            ["Observability:OpenTelemetry:IncludeScopes"] = "true",
            ["Observability:OpenTelemetry:ParseStateValues"] = "true",
            ["Observability:OpenTelemetry:UseMetricsExporter"] = "Console",
            ["Observability:OpenTelemetry:UseTracingExporter"] = "DoNotUse",
            ["Observability:OpenTelemetry:UseLogExporter"] = "DotNotUse",
            ["Observability:OpenTelemetry:HistogramAggregation"] = "Explicit",
            ["Observability:AspNetCoreInstrumentation:RecordException"] = "true",
        };
    }
}
