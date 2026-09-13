# BlogDoFT.Libs.Flagr

HTTP client to evaluate feature flags from a [Flagr](https://github.com/openflagr/flagr) server. Registers `IFlagResolver` (from `BlogDoFT.Libs.Flagr.Abstractions`) as a typed `HttpClient` and evaluates flags by posting to Flagr's `v1/evaluation` endpoint.

Purpose
- Wire up a typed `HttpClient` pointed at a Flagr instance using `IOptions<FlagrOptions>`.
- Resolve a feature flag's variant for a given entity context.

Configuration

```json
{
  "Flagr": {
    "BaseUrl": "https://flagr.example.com/api/"
  }
}
```

Quick start

```csharp
// Composition root
services.AddFlagr();

// or configure options in code
services.AddFlagr(options => options.BaseUrl = "https://flagr.example.com/api/");
```

```csharp
public class MyFeature
{
    private readonly IFlagResolver _flagResolver;

    public MyFeature(IFlagResolver flagResolver)
    {
        _flagResolver = flagResolver;
    }

    public bool IsEnabled()
    {
        var evaluation = _flagResolver.ResolveFlag<MyFeature>(new { });
        return evaluation.VariantKey == "on";
    }
}
```

Notes
- `AddFlagr` fails fast with an `OptionsValidationException` when `Flagr:BaseUrl` is missing or empty.
- To authenticate requests to Flagr with a Keycloak-issued bearer token, add `BlogDoFT.Libs.Keycloak` and attach `KeycloakAuthenticationHandler` to the Flagr `HttpClient` via `AddHttpMessageHandler<KeycloakAuthenticationHandler>()`.
