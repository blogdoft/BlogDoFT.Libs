# Repository Instructions for Copilot

Apply these rules on this repository:
- Language: C# 13 / .NET 9 / nullable enable.
- Follow `/docs/ai/01-coding-standards.md`, `/docs/ai/02-naming-rules.md` and `/docs/ai/03-architecture.md`.
- Tests: xUnit + Shouldly; naming `Should_<Result>_When_<Scenario>`.

When generating code:
- Keep changes minimal and compilable.
- Prefer DI over statics; add cancellation tokens in I/O.
- Never add new packages without justification.

When reviewing:
- Flag nullability issues and missing logs.
