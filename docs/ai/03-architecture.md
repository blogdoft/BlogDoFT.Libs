# Arquitetura
- **Api:** endpoints mínimos + validação FluentValidation.
- **Application:** orquestração (Handlers), contratos, casos de uso.
- **Domain:** entidades, VOs, regras de negócio (sem dependências externas).
- **Infrastructure:** EF Core 9, Migrations, Repositories.

**Decisões:**
- Multi-tenant por `TenantId` (Guid) em todas as entidades de borda.
- Estratégia de filtros: PredicateBuilder; evitar reflexão. Geradores de código quando possível.
