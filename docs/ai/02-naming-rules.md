# Naming Rules

## 1. Solution & projects
- **Solution**: `BlogDoFT.Libs.sln` aggregates libraries only; samples live under `samples/*`.
- **Projects**: `BlogDoFT.Libs.<Context>[.<SubContext>]` (e.g., `BlogDoFT.Libs.DapperUtils.Postgres`). For helper packages (Abstractions, Extensions) keep the suffix explicit.
- **Tests**: `__tests__/BlogDoFT.Libs.<Context>.Tests`. Mirror the same namespace tree as the production project.

## 2. Namespaces & files
- Namespace = logical project path (`namespace BlogDoFT.Libs.ResultPattern;`).
- One file per public type. Internal classes may share a file only when tightly coupled (e.g., `Result` and `Result<T>`).
- Extension files follow `SomethingExtension.cs` and live inside `Extensions/` folders.

## 3. Types & members
- **Interfaces**: prefix with `I` (IDomainNotifications, IConnectionFactory, IWarmUpCommand).
- **DTOs/Commands**: suffix `Dto`, `Request`, `Response`, `Command`, `Query` according to their roles.
- **Records**: use noun-based names (`Failure`, `DomainNotification`).
- **Hosted services**: suffix `HostedService`. Health checks: suffix `HealthCheck`.
- **Async methods**: `Async` suffix, even when returning `Task<Result<T>>` (`ThenAsync`, `TapFailureAsync`).

## 4. Database & migrations (when applicable)
- Migrations: `yyyyMMddHHmm_ShortDescription`.
- Tables/columns: `snake_case` and singular (e.g., `order_item`).
- `Failure.Code` and notification keys: `kebab-case` (`common-404`).

## 5. Tests & data
- Test names: `Should_<Result>_When_<Scenario>`.
- Fakes/fixtures: rely on Bogus/AutoBogus; suffix helpers with `Builder` or `Mother`.
- Private test fields also follow `_camelCase`.

## 6. Docs & scripts
- Files under `docs/ai` start with a numeric prefix for quick reference (`00-overview.md`).
- Shell scripts use `kebab-case` (e.g., `coverage_report.sh`).
