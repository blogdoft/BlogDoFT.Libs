# BlogDoFT.Libs.WarmUp

Health check and warm-up helpers for ASP.NET Core applications. Provides health check implementations and warm-up utilities used by sample apps to report readiness.

Quick start

```csharp
services.AddHealthChecks()
	.AddCheck<WarmUpHealthCheck>("warmup");

app.MapHealthChecks("/health");
```

The `WarmUpHealthCheck` returns healthy only after application warm-up tasks complete (see the sample apps for usage patterns).

## Versioning

Starting with v10.0.0, this library's version numbering follows the major version of the .NET runtime it targets. For example, this version is compatible with .NET 10, so the major version is 10 — this explains the jump from the previous 1.x version series.
