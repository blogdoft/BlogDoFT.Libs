namespace BlogDoFT.Libs.Keycloak;

/// <summary>
/// Configuration options for authenticating against Keycloak with the client
/// credentials OAuth2 flow, typically bound from the "Keycloak" configuration section.
/// </summary>
public class KeycloakOptions
{
    /// <summary>
    /// Gets or sets the Keycloak token endpoint URL.
    /// </summary>
    /// <value>
    /// "https://keycloak.example.com/realms/myrealm/protocol/openid-connect/token"
    /// </value>
    public string TokenEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client identifier registered in Keycloak.
    /// </summary>
    /// <value>
    /// "my-service"
    /// </value>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret registered in Keycloak.
    /// </summary>
    /// <value>
    /// "c3f1b1f0-1234-4a6b-9abc-0123456789ab"
    /// </value>
    public string ClientSecret { get; set; } = string.Empty;
}
