# Testes
- Framework: xUnit + Shouldly; NSubstitute para mocks.
- **Padrão nomes:** `Should_<Resultado>_When_<Cenário>`.
- **Diretórios:** `tests/BlogDoFT.Libs.Tests/`.
- **Cobertura-alvo:** 80% Application/Domain.

**Exemplo:**
```csharp
[Fact] public void Should_CalcTotal_When_OrderHasItems() { /* ... */ }
```
