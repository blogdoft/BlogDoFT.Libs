using BlogDoFT.Libs.Keycloak.Abstractions;
using System.Net.Http.Headers;


namespace BlogDoFT.Libs.Keycloak;

public class KeycloakAuthenticationHandler : DelegatingHandler
{
    private readonly IKeycloakTokenProvider _tokenProvider;

    public KeycloakAuthenticationHandler(IKeycloakTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var accessToken = await _tokenProvider
            .GetAccessTokenAsync(cancellationToken)
            .ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
