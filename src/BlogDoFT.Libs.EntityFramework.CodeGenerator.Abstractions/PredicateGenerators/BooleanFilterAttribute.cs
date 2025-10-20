namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

/// <summary>
/// Declares filter semantics for boolean properties. Always uses equality.
/// </summary>
/// <example>
/// <code>
/// [BooleanFilter(TargetProperty = "Active", Order = 4)]
/// public bool? Active { get; init; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BooleanFilterAttribute : Attribute
{
    /// <summary>
    /// The target property path on the entity. If not set, the DTO property name is used.
    /// </summary>
    public string? TargetProperty { get; set; }

    /// <summary>
    /// Ordering index for composing the final predicate. If not set, declaration order is used.
    /// </summary>
    public int Order { get; set; }
}
