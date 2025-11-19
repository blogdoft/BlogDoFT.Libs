# BlogDoFT.Libs.WarmUp

Health check and warm-up helpers for ASP.NET Core applications. Provides health check implementations and warm-up utilities used by sample apps to report readiness.

Quick start

```csharp
services.AddHealthChecks()
	.AddCheck<WarmUpHealthCheck>("warmup");

app.MapHealthChecks("/health");
```

The `WarmUpHealthCheck` returns healthy only after application warm-up tasks complete (see the sample apps for usage patterns).
