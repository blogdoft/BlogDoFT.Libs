namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

/// <summary>
/// Declares filter semantics for numeric properties.
/// </summary>
/// <example>
/// <code>
/// [NumericFilter(TargetProperty = "Age", Operator = ComparisonOperator.GreaterThanOrEqual, Order = 2)]
/// public int? MinimumAge { get; init; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NumericFilterAttribute : Attribute
{
    /// <summary>
    /// The target property path on the entity. If not set, the DTO property name is used.
    /// </summary>
    public string? TargetProperty { get; set; }

    /// <summary>
    /// Ordering index for composing the final predicate. If not set, declaration order is used.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Comparison operator used to build the expression.
    /// </summary>
    public ComparisonOperator Operator { get; set; } = ComparisonOperator.Equal;
}
