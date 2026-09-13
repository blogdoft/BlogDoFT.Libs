using BlogDoFT.Libs.Keycloak.Abstractions;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BlogDoFT.Libs.Keycloak;

public class KeycloakTokenProvider : IKeycloakTokenProvider
{
    // Renew a little before actual expiry so a request never races a token
    // that's about to be rejected by Keycloak's clock.
    private const int ExpiryBufferSeconds = 30;

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _options;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

    private string _accessToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;

    public KeycloakTokenProvider(HttpClient httpClient, IOptions<KeycloakOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (HasValidToken())
            return _accessToken;

        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (HasValidToken())
                return _accessToken;

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);
            return _accessToken;
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool HasValidToken() =>
        _accessToken is not null && DateTimeOffset.UtcNow < _expiresAt;

    private async Task FetchTokenAsync(CancellationToken cancellationToken)
    {
        var requestedAt = DateTimeOffset.UtcNow;
        using var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
        });

        using var response = await _httpClient
            .PostAsync(_options.TokenEndpoint, content, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var token = await response.Content
            .ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _accessToken = token.AccessToken;
        _expiresAt = requestedAt.AddSeconds(token.ExpiresIn - ExpiryBufferSeconds);
    }
}
