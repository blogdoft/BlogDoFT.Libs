# AI Overview — BlogDoFT.Libs
Updated: 2025-11-08 (TZ America/Sao_Paulo) — .NET 9 / C# 13 / EF Core 9

## 1. Quick context
- This repository hosts reusable libraries consumed by BlogDoFT services (vertical APIs and workers).
- Libraries are small, focused, and independent (ResultPattern, DomainNotifications, Dapper utils, EF Source Generator, WarmUp, Api/Extensions).
- All projects target `net9.0` (except analyzers/source generators on `netstandard2.0`). Nullable is always enabled.
- Packages are published via `dotnet pack` + GitVersion. Do not introduce new dependencies without checking the impact on all consumers.

## 2. Stack & invariants
- **Quality**: Roslynator, StyleCop, SonarAnalyzer, and warnings-as-errors (treat nullability!).
- **Style**: `.editorconfig` + `stylecop.json`; 4 spaces, 120 columns, `#nullable enable` on all public files.
- **Light functional flavor**: heavy use of `Result<T>` + `Failure` and extensions (`Then`, `Map`, `TapFailure`).
- **Explicit DI**: no global singletons; register everything via `IServiceCollection`.
- **I/O & async**: every async method that hits I/O/db receives a `CancellationToken`.
- **Logging**: `ILogger<T>` (or Serilog through `ILoggerFactory`). Structured messages with `SourceContext` = class namespace.

## 3. Library portfolio
| Project | Purpose | Notes |
| --- | --- | --- |
| `BlogDoFT.Libs.ResultPattern` | Implements Result/Failure + chaining extensions. | Keeps flows exception-free and integrates with DomainNotifications. |
| `BlogDoFT.Libs.DomainNotifications` (+ `.Extensions`) | Notification bag with DI + helpers to convert `Result/Failure` into notifications. | Used in APIs to emit consistent errors. |
| `BlogDoFT.Libs.DapperUtils.*` | Testing-friendly abstractions (factories, facades) + Postgres implementation (pagination, where builder, type handlers). | Avoids duplicated SQL and enforces safe pagination. |
| `BlogDoFT.Libs.EntityFramework.CodeGenerator*` | Source generator that emits `HasFilter()`/`ToPredicate()` for annotated DTOs. | Detects at compile time whether Npgsql `ILike` is available. |
| `BlogDoFT.Libs.WarmUp` | Hosted service + health check that fires `IWarmUpCommand` on boot. | Exposes `/health` filtered by the WarmUp tag. |
| `BlogDoFT.Libs.Api` | Helpers for HTTP headers (`X-Correlation-ID`, default language, etc.). | No dependency on full ASP.NET stack. |
| `BlogDoFT.Libs.Extensions` | Utility extensions (e.g., `ReplaceAll`). | Pure code with zero I/O. |
| `samples/*` | Reference projects (e.g., WebApi) showing combined usage. | Run to manually validate breaking changes. |

## 4. Collaboration playbook (AIs & humans)
1. **Know the scope**: identify which library is changing and skim the corresponding `csproj`.
2. **Read the docs** below (coding standards, naming, architecture) before proposing designs.
3. **Keep changes small**: multiple focused PRs beat massive refactors.
4. **Align with tests**: any public change needs coverage inside `__tests__/...` and/or samples.
5. **Explain decisions** in the PR referencing `docs/ai/09-decisions-log.md` when introducing new patterns.

## 5. Relevant documents
- `docs/ai/01-coding-standards.md` — code & quality rules.
- `docs/ai/02-naming-rules.md` — conventions for projects, namespaces, DTOs, and database.
- `docs/ai/03-architecture.md` — how the packages relate.
- `docs/ai/06-testing-strategy.md` — structure, tooling, and coverage goals.
- `docs/ai/07-pr-review-checklist.md` — run through it before opening/closing PRs.

> Whenever these docs conflict with external specs, **these docs win** for this repository.
