namespace BlogDoFT.Libs.Keycloak.Abstractions;

public interface IKeycloakTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
