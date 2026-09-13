using BlogDoFT.Libs.Keycloak.Abstractions;
using BlogDoFT.Libs.Keycloak.Tests.TestSupport;
using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace BlogDoFT.Libs.Keycloak.Tests;

public class AddKeycloakTokenProviderTests
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_ResolveIKeycloakTokenProviderUsingConfiguredEndpoint_When_ConfigurationHasKeycloakSectionAsync()
    {
        // Given
        var tokenEndpoint = $"https://{Faker.Internet.DomainName()}/realms/demo/protocol/openid-connect/token";
        var clientId = Faker.Random.AlphaNumeric(12);
        var clientSecret = Faker.Random.AlphaNumeric(24);
        var accessToken = Faker.Random.AlphaNumeric(32);
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                $$"""{"access_token":"{{accessToken}}","expires_in":300,"token_type":"Bearer"}""",
                Encoding.UTF8,
                "application/json"),
        });
        var services = new ServiceCollection();
        services.AddSingleton(BuildConfiguration(tokenEndpoint, clientId, clientSecret));

        // When
        services.AddKeycloakTokenProvider();
        services
            .AddHttpClient<IKeycloakTokenProvider, KeycloakTokenProvider>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        using var provider = services.BuildServiceProvider();
        var tokenProvider = provider.GetRequiredService<IKeycloakTokenProvider>();
        var token = await tokenProvider.GetAccessTokenAsync();

        // Then
        token.ShouldBe(accessToken);
        handler.LastRequest!.RequestUri!.ToString().ShouldBe(tokenEndpoint);
    }

    [Fact]
    public void Should_ThrowOptionsValidationException_When_KeycloakConfigurationIsIncomplete()
    {
        // Given
        var services = new ServiceCollection();
        services.AddSingleton(BuildConfiguration(
            tokenEndpoint: $"https://{Faker.Internet.DomainName()}/token",
            clientId: null,
            clientSecret: null));
        services.AddKeycloakTokenProvider();
        using var provider = services.BuildServiceProvider();

        // When
        var act = () => provider.GetRequiredService<IKeycloakTokenProvider>();

        // Then
        Should.Throw<OptionsValidationException>(act);
    }

    [Fact]
    public void Should_RegisterKeycloakAuthenticationHandlerAsResolvable_When_UsedToDecorateAnotherHttpClient()
    {
        // Given
        // Regression test: KeycloakAuthenticationHandler must be independently resolvable
        // so other clients (e.g. the Flagr client) can attach it via
        // AddHttpMessageHandler<KeycloakAuthenticationHandler>(), which only resolves the
        // handler and does not register it.
        var services = new ServiceCollection();
        services.AddSingleton(BuildConfiguration(
            tokenEndpoint: $"https://{Faker.Internet.DomainName()}/token",
            clientId: Faker.Random.AlphaNumeric(12),
            clientSecret: Faker.Random.AlphaNumeric(24)));
        services.AddKeycloakTokenProvider();
        services
            .AddHttpClient("downstream-client")
            .AddHttpMessageHandler<KeycloakAuthenticationHandler>();
        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IHttpClientFactory>();

        // When
        var act = () => factory.CreateClient("downstream-client");

        // Then
        Should.NotThrow(act);
    }

    private static IConfiguration BuildConfiguration(string? tokenEndpoint, string? clientId, string? clientSecret)
    {
        var settings = new Dictionary<string, string?>();
        if (tokenEndpoint is not null)
        {
            settings["Keycloak:TokenEndpoint"] = tokenEndpoint;
        }

        if (clientId is not null)
        {
            settings["Keycloak:ClientId"] = clientId;
        }

        if (clientSecret is not null)
        {
            settings["Keycloak:ClientSecret"] = clientSecret;
        }

        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }
}
