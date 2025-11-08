# Prompting Tips (for humans)
- Always paste the relevant snippet or reference files via `@docs/ai/XX-*.md` / `@src/...`. Explicit context saves iterations.
- State the goal and constraints (e.g., "need to expand PaginatedSqlBuilder without breaking the public API").
- Prefer incremental instructions: request one change, validate, then move to the next.
- Ask for diffs or patches instead of whole files. If a full file is needed, explain why.
- Request explanations only when necessary. For simple code, a diff + short summary is enough. For architectural choices, ask for the reasoning.
- When the topic involves patterns defined here, remind the assistant to check `docs/ai/01-03` before proposing solutions.
