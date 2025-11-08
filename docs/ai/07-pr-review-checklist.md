# PR Checklist
- [ ] Build + tests (`dotnet test BlogDoFT.Libs.sln`) pass locally and in CI.
- [ ] No new warnings (nullability, analyzers, StyleCop). Run `dotnet build -warnaserror`.
- [ ] Public changes documented (`docs/ai/*`, README, or XML summary) and, when relevant, logged in `09-decisions-log`.
- [ ] New dependencies justified and aligned across all impacted projects.
- [ ] Structure/naming follows `docs/ai/02-naming-rules.md`.
- [ ] Tests cover positive and negative scenarios.
- [ ] Changes to WarmUp, Dapper builders, or the Source Generator validated in `samples/` manually when applicable.
- [ ] Database migrations/changes come with scripts or clear guidance (if any).
