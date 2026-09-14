using BlogDoFT.Libs.Keycloak.Abstractions;
using System.Net.Http.Headers;

namespace BlogDoFT.Libs.Keycloak;

/// <summary>
/// A <see cref="DelegatingHandler"/> that attaches a Keycloak-issued bearer token to
/// every outgoing HTTP request.
/// </summary>
public class KeycloakAuthenticationHandler : DelegatingHandler
{
    private readonly IKeycloakTokenProvider _tokenProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="tokenProvider">The provider used to obtain access tokens.</param>
    public KeycloakAuthenticationHandler(IKeycloakTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    /// <summary>
    /// Sets the "Authorization" header of <paramref name="request"/> to a Keycloak bearer
    /// token before forwarding it to the inner handler.
    /// </summary>
    /// <param name="request">The outgoing HTTP request.</param>
    /// <param name="cancellationToken">A token to cancel the token retrieval and send.</param>
    /// <returns>The <see cref="HttpResponseMessage"/> returned by the inner handler.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await _tokenProvider
            .GetAccessTokenAsync(cancellationToken)
            ;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}
