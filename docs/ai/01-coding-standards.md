# Coding Standards (C#)

## 1. Technical baseline
- **Target**: .NET 9 / C# 13. Only `BlogDoFT.Libs.EntityFramework.CodeGenerator*` stays on `netstandard2.0` for analyzer compatibility.
- **Nullable** always `enable`; address every warning before committing (`TreatWarningsAsErrors=true` on package projects).
- **Implicit usings** are on; keep the `using` ordering defined in `.editorconfig` (no `System.*` group separation).
- **Required analyzers**: Roslynator (code style/formatting), StyleCop, SonarAnalyzer. Do not disable rules without inline justification or docs.

## 2. Style & readability
- 4 spaces, 120 columns, ASCII files. Prefer file-scoped namespaces and `readonly record struct` for immutable VOs.
- Private fields start with `_` (camelCase). Static fields also use `_` + camelCase per `.editorconfig`.
- Use `var` only when the type is obvious on the same line; otherwise declare explicitly.
- XML comments: only on public APIs (file headers not required). Keep `<summary>` concise and avoid repeating the method name.
- Use `ArgumentNullException.ThrowIfNull()` for mandatory parameter guards.

## 3. Architecture & design
- **Favor immutability**: methods return new values; avoid mutating external state. Prefer records for DTOs/Failures.
- **ResultPattern**: orchestrate flows with `Result<T>` + `Then/Map/TapFailure`. Do not rely on `try/catch` for control flow.
- **DomainNotifications**: when multiple issues must be reported, accumulate them via `IDomainNotifications` instead of throwing.
- **Configuration**: register options via `IOptions<T>`; never read config from static code.
- **Source generators**: for EF filters, use `[GeneratePredicate<TEntity>]` on `partial` DTOs and decorate properties with the proper attributes (`StringFilter`, `NumericFilter`, ...).

## 4. Async, I/O & cancellation
- Append the `Async` suffix to every async method/local function (enforced by `.editorconfig`).
- Any method that performs I/O (database, HTTP, queue) **must** accept a `CancellationToken` propagated from the caller.
- Use `ConfigureAwait(false)` inside libraries (see ResultPattern) to avoid deadlocks.
- For `Task<Result<T>>` pipelines, prefer the provided extensions (`ThenAsync`, `MapAsync`, `TapFailureAsync`) instead of manual `await` + `if` blocks.

## 5. Logging, tracing & errors
- Use `ILogger<T>`; acquire loggers via DI. `WarmUp` already injects `ILoggerFactory` to produce `LogInfo/LogError/LogTrace` delegates.
- Structured messages: `logger.LogInformation("Processing {EntityId}", entity.Id);`.
- When mapping failures, use `Failure` with friendly codes (e.g., `common-404`). Avoid vague error messages.
- Do not expose internal exceptions directly in HTTP responses; convert them into `Result/Failure` + notifications.

## 6. Quick example
```csharp
public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _repository;
    private readonly IDomainNotifications _notifications;

    public CreateOrderHandler(IOrderRepository repository, IDomainNotifications notifications)
    {
        _repository = repository;
        _notifications = notifications;
    }

    public async Task<Result<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await _repository.ExistsAsync(command.OrderNumber, cancellationToken)
            ? Failure.ValidationError
            : await _repository.CreateAsync(command.ToEntity(), cancellationToken);
    }
}
```
