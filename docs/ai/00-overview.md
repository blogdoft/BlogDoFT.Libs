# AI Overview — BlogDoFT.Libs
Atualizado: 2025-11-08 (TZ America/Sao_Paulo) — .NET 9, C# 13, EF Core 9

**Objetivo:** orientar AIs (Claude Code / ChatGPT) sobre como gerar/refatorar código aqui.

**Princípios:**
- Desacoplamento
- Bibliotecas


- Sem “magias” globais: DI explícito, opções via IOptions<T>.
- Nullability **ON**; warnings tratados como erros.
- Evitar reflexão em caminhos quentes; preferir source generators quando fizer sentido.

**Como usar estes docs no VS Code:**
- Claude Code: referencie com `@docs/ai/01-coding-standards.md` ao pedir refactors.
- ChatGPT (extensão): selecione/cole trechos relevantes destes arquivos no prompt.
