using BlogDoFT.Libs.Keycloak;
using BlogDoFT.Libs.Keycloak.Abstractions;
using BlogDoFT.Libs.Keycloak.Tests.TestSupport;
using Bogus;
using System.Net;

namespace BlogDoFT.Libs.Keycloak.Tests;

public class KeycloakAuthenticationHandlerTests
{
    private static readonly Faker Faker = new();

    [Fact]
    public async Task Should_AttachBearerTokenFromProvider_When_SendingRequest()
    {
        // Given
        var accessToken = Faker.Random.AlphaNumeric(32);
        var tokenProvider = Substitute.For<IKeycloakTokenProvider>();
        tokenProvider
            .GetAccessTokenAsync(Arg.Any<CancellationToken>())
            .Returns(accessToken);

        HttpRequestMessage? capturedRequest = null;
        var innerHandler = new FakeHttpMessageHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var handler = new KeycloakAuthenticationHandler(tokenProvider)
        {
            InnerHandler = innerHandler,
        };
        using var client = new HttpClient(handler);

        // When
        await client.GetAsync("http://flagr.test/api/v1/evaluation");

        // Then
        capturedRequest!.Headers.Authorization.ShouldNotBeNull();
        capturedRequest.Headers.Authorization.Scheme.ShouldBe("Bearer");
        capturedRequest.Headers.Authorization.Parameter.ShouldBe(accessToken);
    }
}
