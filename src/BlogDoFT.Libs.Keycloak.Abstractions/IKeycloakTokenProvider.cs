namespace BlogDoFT.Libs.Keycloak.Abstractions;

/// <summary>
/// Provides access tokens issued by a Keycloak server, typically using the client
/// credentials OAuth2 flow.
/// </summary>
public interface IKeycloakTokenProvider
{
    /// <summary>
    /// Gets a valid access token, obtaining a new one from Keycloak if none is cached or
    /// the cached one has expired.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request to Keycloak.</param>
    /// <returns>
    /// The access token to use as a bearer credential, e.g.
    /// "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIn0...".
    /// </returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
