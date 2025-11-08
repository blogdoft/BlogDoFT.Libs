# Coding Standards (C#)
- **Linguagem:** C# 13, .NET 9, nullable enable.
- **Estilo:** Roslynator + StyleCop (SA0001 desabilitada), arquivos `.editorconfig` mandam.
- **Imutabilidade por padrão:** use `record` e `readonly struct` quando adequado.
- **Async:** sufixo `Async`, cancellation token obrigatório em I/O e repositórios.
- **Logs (Serilog):** mensagens estruturadas; `SourceContext` por classe.
- **Linhas de corte:** 120 colunas; arquivos com `#nullable enable`.

**Exemplo:**
```csharp
public sealed class PriceCalculator : IPriceCalculator
{
    private readonly ILogger<PriceCalculator> _logger;
    public PriceCalculator(ILogger<PriceCalculator> logger) => _logger = logger;

    public decimal Calculate(Order order)
    {
        if (order is null) throw new ArgumentNullException(nameof(order));
        // ...
    }
}
```
