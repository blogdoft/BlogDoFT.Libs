using BlogDoFT.Libs.Flagr.Abstractions;
using BlogDoFT.Libs.Flagr.Tests.TestSupport;
using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;

namespace BlogDoFT.Libs.Flagr.Tests;

public class AddFlagrTests
{
    private static readonly Faker Faker = new();

    [Fact]
    public void Should_ResolveIFlagEvaluatorUsingConfiguredBaseUrl_When_ConfigurationHasFlagrSection()
    {
        // Given
        var baseUrl = $"http://{Faker.Internet.DomainName()}/api/";
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}"),
        });
        var services = new ServiceCollection();
        services.AddSingleton(BuildConfiguration(baseUrl));

        // When
        services.AddFlagr();
        services
            .AddHttpClient<IFlagEvaluator, FlagEvaluator>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        using var provider = services.BuildServiceProvider();
        var flagEvaluator = provider.GetRequiredService<IFlagEvaluator>();
        flagEvaluator.EvaluateFlag<SampleEntity>(new { });

        // Then
        handler.LastRequest.ShouldNotBeNull();
        handler.LastRequest.RequestUri!.ToString().ShouldBe($"{baseUrl}v1/evaluation");
    }

    [Fact]
    public void Should_ThrowOptionsValidationException_When_FlagrBaseUrlIsMissing()
    {
        // Given
        var services = new ServiceCollection();
        services.AddSingleton(BuildConfiguration(baseUrl: null));
        services.AddFlagr();
        using var provider = services.BuildServiceProvider();

        // When
        var act = () => provider.GetRequiredService<IFlagEvaluator>();

        // Then
        Should.Throw<OptionsValidationException>(act);
    }

    private static IConfiguration BuildConfiguration(string? baseUrl)
    {
        var settings = baseUrl is null
            ? new Dictionary<string, string?>()
            : new Dictionary<string, string?> { ["Flagr:BaseUrl"] = baseUrl };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    // Never instantiated: used only as a generic type argument to derive the flag key.
#pragma warning disable S2094
    private sealed class SampleEntity;
#pragma warning restore S2094
}
