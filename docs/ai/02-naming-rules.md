# Naming Rules
- **Projetos:** `BlogDoFT.Libs.{Api|Application|Domain|Infrastructure}`
- **Namespaces:** iguais ao caminho lógico do projeto.
- **DTOs:** sufixo `Request`/`Response`.
- **Interfaces:** prefixo `I`, serviços em Application, repositórios em Infrastructure.
- **Migrations:** `yyyyMMddHHmm_DescricaoCurta`.

**Tabelas/cols (EF Core):**
- snake_case para tabelas/colunas; singular para tabela de entidade (ex.: `order_item`).
