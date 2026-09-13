# Libraries & Pinned Versions
> Updated on 2026-09-13. When bumping any package below, sync every affected project and document it in `docs/ai/09-decisions-log.md`.

| Area | Package | Version | Usage |
| --- | --- | --- | --- |
| Micro-ORM | `Dapper` | 2.1.86 | Abstractions + Postgres utilities. |
| Postgres | `Npgsql` | 10.0.3 | Dapper connections (`NpgConnectionFactory`). |
| Config/DI | `Microsoft.Extensions.Configuration` / `Binder` / `Options.ConfigurationExtensions` | 10.0.12 | Bind settings within factories. |
| HTTP util | `Microsoft.AspNetCore.Http` | 2.3.13 | Header helpers in `BlogDoFT.Libs.Api`. |
| Health/Hosted | `Microsoft.AspNetCore.Diagnostics.HealthChecks` | 2.2.0 | WarmUp health check. |
| Hosting | `Microsoft.Extensions.Hosting` / `DependencyInjection` | 10.0.12 | Hosted service and WarmUp composition. |
| Logging | `Microsoft.Extensions.Logging.Abstractions` / `Console` | 10.0.12 | Default logging, also used in the WebApi sample. |
| Observability | `OpenTelemetry.*` (Exporter Console/OTLP/Zipkin, Extensions.Hosting, Instrumentation.AspNetCore/Http/Runtime) | 1.18.0 (`Exporter.Prometheus.AspNetCore` 1.18.0-beta.1, no stable release yet) | `BlogDoFT.Libs.Api.OpenTelemetry`. |
| Validation (sample) | `FluentValidation` | 12.1.1 | Validators in the `WebApi` sample. |
| EF Core (sample) | `Microsoft.EntityFrameworkCore.*` | 10.0.12 | Persistence demo. |
| EF Core Postgres (sample) | `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.3 | EfSamples demo. |
| API docs (sample) | `Swashbuckle.AspNetCore` | 10.2.3 | Swagger UI in the sample. |
| OpenAPI | `Microsoft.AspNetCore.OpenApi` | 10.0.12 | Description generator in the sample. |
| Source generator Roslyn deps | `Microsoft.CodeAnalysis.CSharp` / `Microsoft.CodeAnalysis.Analyzers` | 5.9.0 | `BlogDoFT.Libs.EntityFramework.CodeGenerator` (netstandard2.0). |
| Source generator SourceLink | `Microsoft.SourceLink.GitLab` | 10.0.401 | `BlogDoFT.Libs.EntityFramework.CodeGenerator` (netstandard2.0). |
| Test runner | `Microsoft.NET.Test.Sdk` | 18.10.0 | Test execution. |
| Test libs | `xunit` 2.9.3 / `xunit.runner.visualstudio` 4.0.0 | Testing stack. |
| Assertions | `Shouldly` | 4.3.0 | Fluent assertions. |
| Mocks | `NSubstitute` | 6.2.0 | Mocking in unit tests. |
| Data builders | `Bogus` 35.6.5 / `AutoBogus.NSubstitute` 2.13.1 | Random data generation. |
| Coverage | `coverlet.msbuild` / `coverlet.collector` | 10.0.1 | Coverage reports. |
| Analyzers | `Roslynator.*` 5.0.0, `StyleCop.Analyzers` 1.2.0-beta.556 (newest available, no stable 1.2.0 yet), `SonarAnalyzer.CSharp` 10.34.0.3385 | Style & linting. |

## Rules
- Update versions across all relevant projects (production + tests) to avoid mismatches.
- Packages tagged as "sample" may evolve faster but must remain compatible with the published libraries.
- Always run `dotnet restore && dotnet build -warnaserror` after upgrades.
- `BlogDoFT.Libs.EntityFramework.CodeGenerator*` must stay on `netstandard2.0` — Roslyn analyzers/source generators need to load into any host's compiler process regardless of the consuming project's TFM.
