# BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions

Strongly-typed attributes and enums used by the Predicate Generator. 

These annotations are applied to filter DTOs so a source generator can emit `ToPredicate()` and `HasFilter()` methods.

> **Target Frameworks**: `netstandard2.0`  
> **C# Language Version**: C# 11+ (required for **generic attributes**)

---

## Contents

- `GeneratePredicateAttribute<TEntity>`
- `StringFilterAttribute`
- `NumericFilterAttribute`
- `TemporalFilterAttribute`
- `BooleanFilterAttribute`
- `ComparisonOperator` enum

---

## Installation

Install it in the app project:

```bash
dotnet add package BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions
```

## Usage

Annotate your filter DTO with `[GeneratePredicate<TEntity>]` and mark each filterable property with the respective attribute.

```csharp
using BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions;

[GeneratePredicate<Domain.UserRecord>]
public partial class UserFilterDto
{
    [StringFilter(TargetProperty = nameof(Domain.UserRecord.Name), Order = 1)]
    public string? Name { get; init; }

    [StringFilter(TargetProperty = nameof(Domain.UserRecord.Email), Order = 2)]
    public string? Email { get; init; }

    [NumericFilter(TargetProperty = nameof(Domain.UserRecord.Age),
                   Operator = ComparisonOperator.GreaterThanOrEqual, Order = 3)]
    public int? MinimumAge { get; init; }

    [TemporalFilter(TargetProperty = nameof(Domain.UserRecord.CreatedAt),
                    Operator = ComparisonOperator.LessThanOrEqual, Order = 4)]
    public DateTime? CreatedUntil { get; init; }

    [BooleanFilter(TargetProperty = nameof(Domain.UserRecord.Active), Order = 5)]
    public bool? Active { get; init; }
}
```

>The DTO **must be** `partial`. The generator adds the methods into the same type.

## Behavior

### Generated methods

Generated methods

- `Expression<Func<TEntity, bool>> ToPredicate()`
- `bool HasFilter()`

`HasFilter()` returns `true` if at least one annotated property holds a non-null value.
`ToPredicate()` returns `entity => true` when all annotated values are null; otherwise it combines only the provided filters with logical `AND`.

### String wildcards

When the **string value contains `%`**, the pattern is passed to the database (LIKE):

- No `%` → **equality** (`field == value`)
- Ends with `%` → database pattern (e.g., `value%`)
- Starts with `%` → database pattern (e.g., `%value`)
- `%` in the middle or both sides → database pattern (e.g., `%val%ue%`)

> This relies on the generator’s provider-aware output (see the Generator README).

### Numeric and temporal operators

`NumericFilterAttribute` and `TemporalFilterAttribute` accept `Operator: ComparisonOperator`:

- `Equal`, `GreaterThan`, `LessThan`, `GreaterThanOrEqual`, `LessThanOrEqual`.

### Boolean properties

Always compared by equality.

### Query Order

Usually, the where field orders matters - for database index usage.

Filters are composed by Order (if specified). Otherwise, DTO declaration order is used.

### Requirements

C# 11+ (for generic attributes)

Works with EF Core 6/7/8+ (generator handles provider-specific differences)
