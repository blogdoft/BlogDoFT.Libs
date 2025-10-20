# BlogDoFT.Libs.EntityFramework.CodeGenerator

A Roslyn **Source Generator** that reads filter DTOs annotated with **BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.GeneratePredicateAttribute\<TEntity\>** and emits:

- `ToPredicate()` → `Expression<Func<TEntity, bool>>`
- `HasFilter()` → `bool`

The generated predicates are **provider-aware**:
- On **PostgreSQL** (when `Npgsql.EntityFrameworkCore.PostgreSQL` is referenced), string filters with `%` use **`ILIKE`**.
- On other providers, string filters with `%` use **`EF.Functions.Like`**.
- No reflection is used; the generator detects `NpgsqlDbFunctionsExtensions.ILike` at **compile-time**.

> **Target Framework**: `netstandard2.0` (Analyzer-friendly)  
> **C# Language Version**: latest

---

## Installation

Install the NuGet package:

```bash
dotnet add package BlogDoFT.Libs.EntityFramework.CodeGenerator.Generators
```

>The application must also reference `BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.`

--- 

## How it works

1. The generator scans the compilation for classes annotated with `[GeneratePredicate<TEntity>]`;
2. It validates the DTO is `partial`; otherwise it reports **PG001** (error).
3. It discovers filterable properties annotated with:
    - `StringFilterAttribute`
    - `NumericFilterAttribute`
    - `TemporalFilterAttribute`
    - `BooleanFilterAttribute`
4. It detects at compile-time whether `NpgsqlDbFunctionsExtensions.ILike(DbFunctions, string, string)` is available:
    - If present → emits code that builds expression calls to **ILIKE**
    - If absent → emits code that falls back to **LIKE**
5. It generates partial members `HasFilter()` and `ToPredicate()` into the same DTO namespace and type.

--- 

## Example end-to-end

### Entity

```csharp

namespace Domain;

public class UserRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Active { get; set; }
}
```

### Filter DTO

```csharp
using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions;

[GeneratePredicate<Domain.UserRecord>]
public partial class UserFilterDto
{
    [StringFilter(TargetProperty = nameof(Domain.UserRecord.Name), Order = 1)]
    public string? Name { get; init; }

    [StringFilter(TargetProperty = nameof(Domain.UserRecord.Email), Order = 2)]
    public string? Email { get; init; }

    [NumericFilter(TargetProperty = nameof(Domain.UserRecord.Age), Operator = ComparisonOperator.GreaterThanOrEqual)]
    public int? MinimumAge { get; init; }

    [TemporalFilter(TargetProperty = nameof(Domain.UserRecord.CreatedAt), Operator = ComparisonOperator.LessThanOrEqual)]
    public DateTime? CreatedUntil { get; init; }

    [BooleanFilter(TargetProperty = nameof(Domain.UserRecord.Active))]
    public bool? Active { get; init; }
}

```

### Query with EF Core

```csharp
var filter = new UserFilterDto
{
    Name = "%ann%",
    MinimumAge = 18,
    Active = true
};

var predicate = filter.ToPredicate();

var users = await db.Set<UserRecord>()
    .Where(predicate)
    .ToListAsync();
```

---

## String pattern rules

- If the value **does not contain** `%`, the generator emits **equality** (`field == value`).
- If the value **contains** `%`, the generator emits a database pattern using:
    - `ILIKE` (PostgreSQL with Npgsql)
    - `LIKE` (fallback on other providers)
This way you keep multi-database compatibility while getting case-insensitive matching on PostgreSQL.
>If you want to support `*` as a wildcard in input, normalize it to `%` in your DTO before calling `ToPredicate()`.

---

## Requirements

- BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions must be referenced by the app
- C# 11+ (generic attributes) in the app project
- EF Core 6/7/8+ 

--- 

## Troubleshooting

### The generated methods are missing
- Ensure the DTO is marked as `partial`;
- Ensure the app references BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions and the generator as Analyzer.
- Clean and rebuild the solution.

### I want to see SQL logs

Install and configure console logging:
```xml
<PackageReference Include="Microsoft.Extensions.Logging.Console" />
```

```csharp
services.AddLogging(b =>
{
    b.ClearProviders();
    b.AddSimpleConsole();
    b.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
});
```

Add in your DbContext Options:

```csharp 
options
    .UseLoggerFactory(loggerFactory)
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors();

```