# Libraries & Pinned Versions
> Updated on 2025-11-08. When bumping any package below, sync every affected project and document it in `docs/ai/09-decisions-log.md`.

| Area | Package | Version | Usage |
| --- | --- | --- | --- |
| Micro-ORM | `Dapper` | 2.1.66 | Abstractions + Postgres utilities. |
| Postgres | `Npgsql` | 9.0.4 | Dapper connections (`NpgConnectionFactory`). |
| Config/DI | `Microsoft.Extensions.Configuration` / `Binder` / `Options.ConfigurationExtensions` | 9.0.10 | Bind settings within factories. |
| HTTP util | `Microsoft.AspNetCore.Http` | 2.3.0 | Header helpers in `BlogDoFT.Libs.Api`. |
| Health/Hosted | `Microsoft.AspNetCore.Diagnostics.HealthChecks` | 2.2.0 | WarmUp health check. |
| Hosting | `Microsoft.Extensions.Hosting` / `DependencyInjection` | 9.0.10 | Hosted service and WarmUp composition. |
| Logging | `Microsoft.Extensions.Logging.Abstractions` / `Console` | 9.0.10 | Default logging, also used in the WebApi sample. |
| Validation (sample) | `FluentValidation` | 12.1.0 | Validators in the `WebApi` sample. |
| EF Core (sample) | `Microsoft.EntityFrameworkCore.*` | 9.0.10 | Persistence demo. |
| API docs (sample) | `Swashbuckle.AspNetCore` | 9.0.6 | Swagger UI in the sample. |
| OpenAPI | `Microsoft.AspNetCore.OpenApi` | 9.0.6 | Description generator in the sample. |
| Test runner | `Microsoft.NET.Test.Sdk` | 18.0.0 | Test execution. |
| Test libs | `xunit` 2.9.3 / `xunit.runner.visualstudio` 3.1.5 | Testing stack. |
| Assertions | `Shouldly` | 4.3.0 | Fluent assertions. |
| Mocks | `NSubstitute` | 5.3.0 | Mocking in unit tests. |
| Data builders | `Bogus` 35.6.5 / `AutoBogus.NSubstitute` 2.13.1 | Random data generation. |
| Coverage | `coverlet.msbuild` / `coverlet.collector` | 6.0.4 | Coverage reports. |
| Analyzers | `Roslynator.*` 4.14.1, `StyleCop.Analyzers` 1.1.118 (1.2.0-beta.556 on WarmUp), `SonarAnalyzer.CSharp` 10.15.0.120848 | Style & linting. |

## Rules
- Update versions across all relevant projects (production + tests) to avoid mismatches.
- Packages tagged as "sample" may evolve faster but must remain compatible with the published libraries.
- Always run `dotnet restore && dotnet build -warnaserror` after upgrades.
