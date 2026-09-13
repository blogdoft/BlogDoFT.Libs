# BlogDoFT.Libs.Keycloak.Abstractions

Contracts for obtaining Keycloak access tokens. Defines `IKeycloakTokenProvider`, so consumers can depend on an abstraction instead of the token-acquisition implementation.

Purpose
- Keep Keycloak token-acquisition contracts decoupled from the HTTP implementation.
- Allow other libraries (e.g. `BlogDoFT.Libs.Flagr`) and application code to authenticate outgoing requests without depending on Keycloak-specific details.

Usage

Reference this package when you only need to consume `IKeycloakTokenProvider` (e.g. in application/domain code), and add `BlogDoFT.Libs.Keycloak` in the composition root to provide the implementation.

```csharp
public class MyService
{
    private readonly IKeycloakTokenProvider _tokenProvider;

    public MyService(IKeycloakTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken) =>
        await _tokenProvider.GetAccessTokenAsync(cancellationToken);
}
```
