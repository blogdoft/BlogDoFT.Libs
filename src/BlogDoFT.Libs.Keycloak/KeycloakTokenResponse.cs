using System.Text.Json.Serialization;

namespace BlogDoFT.Libs.Keycloak;

internal class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
}
