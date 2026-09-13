# BlogDoFT.Libs.DapperUtils.Abstractions

Abstractions and contracts for Dapper-based data access helpers. Defines interfaces, common DTOs and small builders used by Postgres implementation.

Purpose
- Keep data-access contracts decoupled from implementation.
- Provide paging and filtering primitives used across Dapper utilities.

Usage
Reference this package from your data layer implementation and use the interfaces to program against the abstractions.

## Versioning

Starting with v10.0.0, this library's version numbering follows the major version of the .NET runtime it targets. For example, this version is compatible with .NET 10, so the major version is 10 — this explains the jump from the previous 1.x version series.
