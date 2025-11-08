# ADR (Decisions)
- **2025-11-08** — All libraries in the repository must stay on .NET 9 (except generators on `netstandard2.0`). Updated the AI docs and consolidated instructions in this directory.
- **2025-09-22** — `BlogDoFT.Libs.EntityFramework.CodeGenerator` detects `NpgsqlDbFunctionsExtensions.ILike` at compile time; when unavailable, it generates `EF.Functions.Like`. Reflection-free EF filters only.
- **2025-08-15** — WarmUp exposes a dedicated health check with its own tag, returning `HealthStatus.Degraded` until every `IWarmUpCommand` completes.
- **2025-07-03** — `DomainNotifications.Extensions` became the official integration between `ResultPattern` and notifications. Always rely on these helpers before mapping manually.
- **2025-05-10** — Dapper paginators (`PaginatedSqlBuilder`) must split the main query from the count query to prevent SQL injection and enable reuse on dashboards.
