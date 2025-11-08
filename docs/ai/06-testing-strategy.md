# Testing Strategy

## 1. Tooling
- **Framework**: xUnit + Shouldly.
- **Mocks/Fakes**: NSubstitute for dependencies, Bogus/AutoBogus for data.
- **Coverage**: Coverlet (`dotnet test /p:CollectCoverage=true`). Helper script: `./coverage_report.sh`.

## 2. Layout
- All tests live under `__tests__/BlogDoFT.Libs.<Project>.Tests`. Mirror the same folder/namespace structure as the target project.
- Organize by concern (e.g., `ResultExtensionTests/ResultExtensionOnAsyncTests.cs`).
- Keep expected data inline or in small builders within the same file. Extract complex helpers to nested `Fixture` classes.

## 3. Writing conventions
- Method name: `Should_<Result>_When_<Scenario>`.
- Arrange/Act/Assert indicated via comments or blank lines. Prefer `// Given // When // Then` like the existing tests.
- Never use `async void`; always return `Task`.
- Every new public API needs positive and negative coverage.

## 4. Goals
- **Minimum coverage**: 80% for Application/Domain/Lib projects. Pay attention to failure paths in ResultPattern and builders.
- **Performance**: keep tests deterministic; use `Bogus` for non-critical data and set seeds when needed (`new Faker(locale).UseSeed(...)`).

## 5. Commands
```bash
# run everything
DOTNET_ENVIRONMENT=Test dotnet test BlogDoFT.Libs.sln

# consolidate coverage
./coverage_report.sh
```

## 6. When to add tests
- Any change in ResultPattern, notifications, builders, or WarmUp behavior demands new scenarios.
- Before refactoring, cover the current behavior to guard against regressions.
- Samples are for manual smoke tests only; they do not replace automated coverage.
