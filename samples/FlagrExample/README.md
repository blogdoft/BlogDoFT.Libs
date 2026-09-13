# Flagr Example

Reference sample showing how to wire up `BlogDoFT.Libs.Flagr` and `BlogDoFT.Libs.Keycloak` in a console app to resolve a feature flag from a [Flagr](https://github.com/openflagr/flagr) server, authenticating through Keycloak.

This sample is **not** part of `BlogDoFT.Libs.slnx` — it has its own solution and exists purely as a reference / learning aid; it is not built or tested by the main CI pipeline.

Originally published alongside this article: https://www.blogdoft.com.br/index.php/2020/06/26/toggle-features-e-feature-flags-com-flagr/

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A running [Flagr](https://github.com/openflagr/flagr) instance (see `.docker/docker-compose.yml` for a local one)

## Project structure

- `src/FlagrExample` — the console app
- `tests/FlagrExample.Tests` — xUnit test suite (`dotnet test`)

## Configuration

Settings live in `src/FlagrExample/appsettings.json`:

```json
{
  "Flagr": {
    "BaseUrl": "https://your-flagr-host/api/"
  },
  "Keycloak": {
    "TokenEndpoint": "https://your-keycloak-host/realms/<realm>/protocol/openid-connect/token",
    "ClientId": "<client-id>",
    "ClientSecret": "<client-secret>"
  }
}
```

The app authenticates against Flagr using the OAuth2 **client credentials** grant through Keycloak, so `ClientId` needs to be a confidential client with "Service Accounts" enabled.

Don't put real credentials in `appsettings.json` — instead, override them locally with one of:

- **.NET user secrets** (recommended for local dev):
  ```
  dotnet user-secrets set "Keycloak:ClientSecret" "<value>" --project src/FlagrExample
  ```
- `src/FlagrExample/appsettings.Local.json` (gitignored)
- Environment variables (e.g. `Keycloak__ClientSecret`)

If Flagr's evaluation endpoint isn't behind Keycloak, just point `Flagr:BaseUrl` at it and leave the `Keycloak` section as-is — the app will fail to fetch a token, which only matters if the endpoint actually challenges it.

## Running

```
dotnet run --project src/FlagrExample
```

## Testing

```
dotnet test FlagrExample.slnx
```
