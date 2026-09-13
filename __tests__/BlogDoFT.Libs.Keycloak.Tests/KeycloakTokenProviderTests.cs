using BlogDoFT.Libs.Keycloak;
using BlogDoFT.Libs.Keycloak.Tests.TestSupport;
using Bogus;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace BlogDoFT.Libs.Keycloak.Tests;

public class KeycloakTokenProviderTests
{
    private static readonly Faker Faker = new();

    private static KeycloakOptions CreateOptions() => new()
    {
        TokenEndpoint = $"https://{Faker.Internet.DomainName()}/realms/demo/protocol/openid-connect/token",
        ClientId = Faker.Random.AlphaNumeric(12),
        ClientSecret = Faker.Random.AlphaNumeric(24),
    };

    private static HttpResponseMessage TokenResponse(string accessToken, int expiresIn) =>
        new(HttpStatusCode.OK)
        {
            Content = new StringContent(
                $$"""{"access_token":"{{accessToken}}","expires_in":{{expiresIn}},"token_type":"Bearer"}""",
                Encoding.UTF8,
                "application/json"),
        };

    [Fact]
    public async Task Should_RequestTokenUsingClientCredentialsGrant_When_NoTokenIsCached()
    {
        // Given
        var options = CreateOptions();
        var accessToken = Faker.Random.AlphaNumeric(32);
        var handler = new FakeHttpMessageHandler(_ => TokenResponse(accessToken, expiresIn: 300));
        var provider = new KeycloakTokenProvider(new HttpClient(handler), Options.Create(options));

        // When
        var token = await provider.GetAccessTokenAsync();

        // Then
        token.ShouldBe(accessToken);
        handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
        handler.LastRequest.RequestUri!.ToString().ShouldBe(options.TokenEndpoint);
        handler.LastRequest.Content!.Headers.ContentType!.MediaType.ShouldBe("application/x-www-form-urlencoded");
        handler.LastRequestBody.ShouldNotBeNull();
        handler.LastRequestBody.ShouldContain("grant_type=client_credentials");
        handler.LastRequestBody.ShouldContain($"client_id={options.ClientId}");
        handler.LastRequestBody.ShouldContain($"client_secret={options.ClientSecret}");
    }

    [Fact]
    public async Task Should_ReuseCachedToken_When_RequestedAgainBeforeExpiry()
    {
        // Given
        var options = CreateOptions();
        var accessToken = Faker.Random.AlphaNumeric(32);
        var callCount = 0;
        var handler = new FakeHttpMessageHandler(_ =>
        {
            callCount++;
            return TokenResponse(accessToken, expiresIn: 300);
        });
        var provider = new KeycloakTokenProvider(new HttpClient(handler), Options.Create(options));

        // When
        var first = await provider.GetAccessTokenAsync();
        var second = await provider.GetAccessTokenAsync();

        // Then
        first.ShouldBe(accessToken);
        second.ShouldBe(accessToken);
        callCount.ShouldBe(1);
    }

    [Fact]
    public async Task Should_FetchNewToken_When_PreviousTokenHasExpired()
    {
        // Given
        var options = CreateOptions();
        var firstToken = Faker.Random.AlphaNumeric(32);
        var secondToken = Faker.Random.AlphaNumeric(32);
        var callCount = 0;
        var handler = new FakeHttpMessageHandler(_ =>
        {
            callCount++;
            return callCount == 1
                ? TokenResponse(firstToken, expiresIn: 0)
                : TokenResponse(secondToken, expiresIn: 300);
        });
        var provider = new KeycloakTokenProvider(new HttpClient(handler), Options.Create(options));

        // When
        var first = await provider.GetAccessTokenAsync();
        var second = await provider.GetAccessTokenAsync();

        // Then
        first.ShouldBe(firstToken);
        second.ShouldBe(secondToken);
        callCount.ShouldBe(2);
    }
}
