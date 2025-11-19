# BlogDoFT.Libs.DapperUtils.Abstractions

Abstractions and contracts for Dapper-based data access helpers. Defines interfaces, common DTOs and small builders used by Postgres implementation.

Purpose
- Keep data-access contracts decoupled from implementation.
- Provide paging and filtering primitives used across Dapper utilities.

Usage
Reference this package from your data layer implementation and use the interfaces to program against the abstractions.
