# Repository Instructions for Copilot

## Baseline
- Projects: C# 13 / .NET 9 / `nullable enable`. Generators/analyzers run on `netstandard2.0`.
- Always read `docs/ai/00-overview.md` plus `01-coding-standards.md`, `02-naming-rules.md`, `03-architecture.md` before suggesting code.
- Tests live in `__tests__/BlogDoFT.Libs.<Project>.Tests` with xUnit + Shouldly + NSubstitute.

## When generating code
- Reuse existing extensions (`ResultPattern`, `DomainNotifications`, Dapper builders) instead of reinventing flows.
- Enforce explicit DI: no static singletons; expose services through `IServiceCollection`.
- For I/O/async, add `CancellationToken` parameters and use `ConfigureAwait(false)` inside libraries.
- Follow the naming/doc conventions in `docs/ai/02-naming-rules.md`.
- Avoid adding new packages; if unavoidable, explain why and update `docs/ai/05-libraries-and-versions.md`.

## When reviewing code
- Check nullability, proper `Result/Failure` usage, and sufficient logging on error paths.
- Ensure tests cover main scenarios and naming follows the standards.
- Confirm public-facing changes are reflected in the relevant docs or the decisions log.
