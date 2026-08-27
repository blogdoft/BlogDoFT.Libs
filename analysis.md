# Architecture & Design Review — BlogDoFT.Libs

Consolidated review across all 11 library projects under `src/`, prioritized by impact.

## 🔴 High-impact design flaws (will bite in production)

**WarmUp's "warm up" doesn't actually wait for warm-up** — `WarmUpExecutor.Execute()` and `PreloadingCommand.Execute()` return `Task.CompletedTask` while the real work runs in a detached, unawaited `Task.Run`. `WarmUpHostedService`'s `await preloading.Execute(); await warmUpExecutor.Execute();` looks sequential but isn't — both background jobs actually race against each other and against the app becoming "ready." (`WarmUpExecutor.cs:26-49`, `PreloadingCommand.cs:23-41`)

**EF source generator's error-reporting story is fake** — 8 of the 9 `DiagnosticDescriptor`s (`TargetPropertyPathInvalid`, `UnsupportedFilterAttribute`, `EntityTypeNotResolved`, etc.) are never actually reported anywhere in `PredicateGenerator.cs` — only `FilterDtoMustBePartial` is used. Invalid attribute usage currently produces silently wrong or non-compiling generated code instead of a build-time diagnostic, which defeats the whole point of a source generator's fail-safe design.

**`ComparisonOperator` handling can silently generate wrong code** — the generator hardcodes the enum as raw `int` literals (`case 0..4`) in a switch disconnected from `Abstractions.ComparisonOperator`, with an unknown value silently falling back to `Equal`. Adding a new operator compiles fine and generates incorrect filters with no warning. (`PredicateGenerator.cs:193-204`)

**`WarmUpServiceExtension.AddWarmUp` builds a provider mid-registration** — calls `services.BuildServiceProvider()` just to grab an `ILoggerFactory`, then discards it — the classic ASP0000 anti-pattern against a partially-configured container. It also reinvents logging via `Action<string>` delegates instead of `ILogger<T>`, requiring `CA2254` to be suppressed project-wide.

**`PostgresDatabaseFacade` opens a DB connection synchronously in its constructor**, and it's registered `Scoped` — every scope creation blocks on a live round-trip whether or not the facade is used, in an otherwise fully-async API.

## 🟠 Competing patterns causing real confusion

**`Result<T>` and `IDomainNotifications` are two overlapping "did this fail and why" mechanisms**, and the sample app proves it's confusing in practice: `SalesCreate`/`SalesQuery` write every failure into the notifications bag (`.TapFailure(_notifications.Add)`), but `SalesController` builds its entire HTTP response from the returned `Result` alone — the notifications bag is **never read**. It's a dead side channel that exists only because `Failure` (ResultPattern) and `DomainNotification` (DomainNotifications) duplicate the same "code + message" concept with different nullability contracts, forcing an entire bridge project (`DomainNotifications.Extensions`) just to convert between them.

## 🟡 Leaky abstractions / layering

- `DapperUtils.Abstractions` directly depends on the `Dapper` package and wraps `Dapper.SqlMapper.GridReader` — it isn't actually ORM-agnostic despite the name.
- `WhereBuilder` and `OrderByResolver` emit 100% vendor-neutral ANSI SQL but live in `DapperUtils.Postgres`, not `.Abstractions` — a future SQL Server provider can't reuse them.
- `NpgConnectionFactory` re-registers Dapper's global type handlers on *every* `GetNewConnection()` call, and hardcodes the connection-string key and schema (`"public"`) with no override hook.
- `DapperUtils.Postgres` (a low-level, provider-specific package) depends sideways on the generic `BlogDoFT.Libs.Extensions` grab-bag.
- `PreloadingCommand.GetServices()` resolves **every** registered service in the container at startup — a blunt, hidden side effect with no opt-in list; combined with `WarmUpExecutor`'s reflection over the build-time `IServiceCollection`, WarmUp implements a service-locator pattern instead of idiomatic `IEnumerable<IWarmUpCommand>` DI resolution.

## 🟢 Extensibility (Open/Closed violations)

- `PredicateGenerator` is a single 359-line class mixing pipeline wiring, symbol analysis, and string-builder code emission with no seam for unit testing without running the full Roslyn pipeline.
- Filter-kind dispatch is a hardcoded `if/else` chain on attribute name string, not pluggable — adding a 5th filter attribute means editing this core method.
- The generator identifies `[GeneratePredicate<T>]` via `attributeClass.Name.StartsWith(...)` rather than resolving the real symbol — a same-named unrelated attribute would false-positive match.

## 🔵 Solution-level structure

- **Lockstep versioning**: CI stamps every one of the 11 independently-packable NuGet packages with the same GitVersion output on every publish — a one-line fix in `WarmUp` republishes all 11 packages under an identical version bump, misleading consumers about what actually changed.
- **Test coverage blind spot**: `EntityFramework.CodeGenerator` and its `.Abstractions` project have *no* test project at all — exactly the code (a source generator) where design regressions go unnoticed silently. `BlogDoFT.Libs.Api.Tests` exists in the solution but contains zero actual tests, a stub giving false CI confidence.
- **Samples under-exercise the library set**: the WebApi sample only wires up `ResultPattern`/`DomainNotifications`/`Extensions`; `Api`, `Api.OpenTelemetry`, `DapperUtils.*`, and `WarmUp` have no real "how do I wire this into an app" reference anywhere in the repo — including the WarmUp sequencing bug and the OTel `UseOpenTelemetry` silent-no-op-on-wrong-type issue below, neither of which would ever surface in the repo's own sample.

## ⚪ Minor / API contract inconsistencies

- `EnumExtensions.ToEnum<TEnum>` throws `InvalidCastException` on a parse failure — semantically should be `FormatException`/`ArgumentException` per `Enum.Parse` convention, so consumers catching the idiomatic exception type will miss it.
- `StringExtensions.ReplaceAll` recurses with no cycle guard — if `newString` contains `oldString` (e.g. replacing `"a"` with `"aa"`), it recurses until stack overflow.
- `ResultException.CallFailureOnSuccess()` is defined but never invoked, and `Result<TValue>.Value`'s success/failure guard uses `ReferenceEquals(Failure, Failure.None)` while `IsFailure` uses record value equality — two different, inconsistent ways of enforcing the same "IsSuccess implies Value is safe" invariant. A hand-built `Failure` equal-by-value to `None` but not reference-equal would report `IsSuccess == true` yet still throw on `.Value`.
- `PostgresDatabaseFacade.QueryFirstAsync<T>` exists on the concrete internal class but isn't declared on `IDatabaseFacade` — since the class is `internal`, this is unreachable dead API surface.
- `OpenTelemetryExtension.UseOpenTelemetry` does `(app as IEndpointRouteBuilder)?.MapPrometheusScrapingEndpoint()` — a silent no-op if the caller passes a plain `IApplicationBuilder` rather than `WebApplication`, with no error or log on misconfiguration.
- `Observability` manually parses `IConfiguration` in its constructor and is registered via `AddSingleton(Options.Create(...))`, bypassing the standard `services.Configure<T>()`/`IOptionsMonitor`/`IValidateOptions` pipeline, while exposing mutable `{ get; set; }` properties on what's effectively shared singleton state.

## What's actually solid

- Project dependency graph has no cycles or inverted references (`Postgres → Abstractions`, `CodeGenerator → CodeGenerator.Abstractions` both flow correctly).
- `IConnectionFactory`/`IDatabaseFacade`/`IGridReaderFacade` are narrow, mockable interfaces (good ISP).
- `Then`/`Map`/`Tap`/`TapFailure` in ResultPattern correctly and consistently short-circuit on failure — solid monadic composition.
- `EntityFramework.CodeGenerator.Abstractions` attributes are pure markers with zero generator logic leaking in — clean split.
- `Api` vs `Extensions` split is justified (framework-dependent vs. framework-agnostic), not arbitrary.

---

*Next step suggestion: the WarmUp sequencing bug and the dead diagnostics in the source generator are probably the highest-value places to start fixing.*
