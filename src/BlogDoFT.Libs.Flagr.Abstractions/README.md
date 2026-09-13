# BlogDoFT.Libs.Flagr.Abstractions

Contracts and DTOs for resolving feature flags from [Flagr](https://github.com/openflagr/flagr). Defines `IFlagEvaluator` and the evaluation types used by the Flagr client implementation, so consumers can depend on an abstraction instead of the HTTP client details.

Purpose
- Keep feature-flag evaluation contracts decoupled from the HTTP implementation.
- Provide the `EvaluationResponse` / `EvalContext` shapes returned by Flagr's evaluation API.

Usage

Reference this package when you only need to consume `IFlagEvaluator` (e.g. in application/domain code), and add `BlogDoFT.Libs.Flagr` in the composition root to provide the implementation.

```csharp
public class MyFeature
{
    private readonly IFlagEvaluator _FlagEvaluator;

    public MyFeature(IFlagEvaluator FlagEvaluator)
    {
        _FlagEvaluator = FlagEvaluator;
    }

    public bool IsEnabled()
    {
        var evaluation = _FlagEvaluator.EvaluateFlag<MyFeature>(new { });
        return evaluation.VariantKey == "on";
    }
}
```

## Versioning

Starting with v10.0.0, this library's version numbering follows the major version of the .NET runtime it targets. For example, this version is compatible with .NET 10, so the major version is 10 — this explains the jump from the previous 1.x version series.
