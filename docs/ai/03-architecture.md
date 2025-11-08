# Architecture

## 1. Overview
- The repo contains **decoupled libraries** consumed by Web APIs, workers, and CLIs at BlogDoFT. Each folder in `src/` becomes an independent NuGet package.
- There is no main app here; `samples/` demonstrates real integrations and `__tests__/` keeps behavior stable.
- `Directory.Build.props` centralizes metadata (GitVersion, InformationalVersion) so every package ships consistent versions.

## 2. Core components
### ResultPattern + DomainNotifications
- `Result`/`Result<T>` wrap success/failure and expose `Failure` (record with `Code` + `Message`).
- Extensions (`Then`, `Map`, `TapFailure`, `On`) enable sync/async composition without repetitive `try/catch` blocks.
- `DomainNotifications` provides a scoped bag registered via `IServiceCollection`, while `DomainNotifications.Extensions` translates `Result/Failure/Exception` into notifications.
- Usage pattern: handlers return `Result<T>`, controllers translate to HTTP responses and add notifications when required.

### Dapper Utils (Abstractions + Postgres)
- **Abstractions**: contracts for `IConnectionFactory`, `IDatabaseFacade`, `IGridReaderFacade`, pagination (`PageFilter`), type handlers (`SqlDateOnly/TimeOnly`).
- **Postgres**: concrete implementation with `Npgsql` + `Dapper`. Highlights:
  - `PaginatedSqlBuilder` builds paged SELECTs (query + querySize) with validations.
  - `WhereBuilder`, `OrderByResolver`, and `SqlPagination` encapsulate safe SQL generation.
  - `NpgConnectionFactory` reads connection strings via `IOptions<T>`.

### EntityFramework Code Generator
- `BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions` defines attributes (`GeneratePredicate`, `StringFilter`, `NumericFilter`, `TemporalFilter`, `BooleanFilter`).
- The `Generators` project scans annotated `partial` DTOs and emits `HasFilter()` / `ToPredicate()` with multi-provider awareness (detects `NpgsqlDbFunctionsExtensions.ILike`).
- Goal: eliminate reflection and handwritten `Expression<Func<T,bool>>` when filtering entities.

### WarmUp
- `IWarmUpCommand` defines a unit of work executed during startup.
- `WarmUpHostedService` calls `WarmUpExecutor`, which resolves every registered command and toggles `WarmUpHealthCheck` once done.
- `WarmUpServiceExtension` wires the hosted service, tagged health check, and exposes `UseWarmUp(route)` to publish the JSON status endpoint.

### Api + Extensions
- `BlogDoFT.Libs.Api` contains HTTP header helpers (`X-Correlation-ID`, forwarded host, default language) without bringing the full ASP.NET stack.
- `BlogDoFT.Libs.Extensions` stores pure helpers (strings, enums) with zero external dependencies.

### Samples & tests
- `samples/WebApi` shows how to combine ResultPattern + DomainNotifications + Validators + EF Core.
- The `__tests__` folder mirrors production projects and uses xUnit + Shouldly + Bogus/NSubstitute. Coverage is gathered by `dotnet test /p:CollectCoverage=true` or `coverage_report.sh`.

## 3. Cross-dependencies
- `DomainNotifications.Extensions` references `ResultPattern`.
- `DapperUtils.Postgres` depends on `DapperUtils.Abstractions` and `BlogDoFT.Libs.Extensions`.
- Samples reference multiple libraries for integration exercises, but packages under `src/` stay independent (except for the pairs above).
- Tests reference their respective projects + required utilities.

## 4. Evolution guidelines
- Avoid circular dependencies between libraries. If contracts must be shared, create a dedicated `*.Abstractions` package.
- When introducing new cross-cutting capabilities, add a new project under `src/BlogDoFT.Libs.<Name>` plus matching tests, keeping explicit cohesion in the docs.
