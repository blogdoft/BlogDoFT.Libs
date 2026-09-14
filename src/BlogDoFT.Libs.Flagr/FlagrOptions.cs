namespace BlogDoFT.Libs.Flagr;

/// <summary>
/// Configuration options for connecting to a Flagr server, typically bound from the
/// "Flagr" configuration section.
/// </summary>
public class FlagrOptions
{
    /// <summary>
    /// Gets or sets the base URL of the Flagr server.
    /// </summary>
    /// <value>
    /// "https://flagr.example.com"
    /// </value>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether requests to Flagr should be authenticated with a Keycloak
    /// bearer token.
    /// </summary>
    /// <value>
    /// true
    /// </value>
    public bool KeycloakAuthenticate { get; set; } = true;
}
