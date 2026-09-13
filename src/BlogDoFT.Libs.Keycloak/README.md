# BlogDoFT.Libs.Keycloak

Client-credentials Keycloak token provider and a `DelegatingHandler` to attach `Bearer` tokens to outgoing HTTP requests. Registers `IKeycloakTokenProvider` (from `BlogDoFT.Libs.Keycloak.Abstractions`) as a typed `HttpClient` and caches the access token until shortly before it expires.

Purpose
- Fetch and cache an access token from Keycloak using the `client_credentials` grant.
- Provide `KeycloakAuthenticationHandler`, a `DelegatingHandler` that attaches the cached token to any `HttpClient` it decorates.

Configuration

```json
{
  "Keycloak": {
    "TokenEndpoint": "https://keycloak.example.com/realms/demo/protocol/openid-connect/token",
    "ClientId": "my-client",
    "ClientSecret": "my-secret"
  }
}
```

Quick start

```csharp
// Composition root
services.AddKeycloakTokenProvider();

// or configure options in code
services.AddKeycloakTokenProvider(options =>
{
    options.TokenEndpoint = "https://keycloak.example.com/realms/demo/protocol/openid-connect/token";
    options.ClientId = "my-client";
    options.ClientSecret = "my-secret";
});
```

Authenticating another `HttpClient`

```csharp
services
    .AddHttpClient("downstream-client")
    .AddHttpMessageHandler<KeycloakAuthenticationHandler>();
```

Notes
- `AddKeycloakTokenProvider` fails fast with an `OptionsValidationException` when `Keycloak:TokenEndpoint`, `Keycloak:ClientId` or `Keycloak:ClientSecret` is missing or empty.
- `KeycloakAuthenticationHandler` is registered so it can be attached to other `HttpClient` registrations (e.g. the Flagr client from `BlogDoFT.Libs.Flagr`) via `AddHttpMessageHandler<KeycloakAuthenticationHandler>()`; it is never automatically applied to the token-provider's own client, since fetching a token cannot depend on already having one.

## Versioning

Starting with v10.0.0, this library's version numbering follows the major version of the .NET runtime it targets. For example, this version is compatible with .NET 10, so the major version is 10 — this explains the jump from the previous 1.x version series.
