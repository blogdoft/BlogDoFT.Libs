namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

/// <summary>
/// Declares filter semantics for string properties.
/// </summary>
/// <remarks>
/// Matching behavior follows the wildcard rules:
/// <list type="bullet">
/// <item><description>No <c>%</c>: equality</description></item>
/// <item><description>Ends with <c>%</c>: <c>StartsWith</c></description></item>
/// <item><description>Starts with <c>%</c>: <c>EndsWith</c></description></item>
/// <item><description><c>%</c> in the middle (or on both sides): <c>Contains</c></description></item>
/// </list>
/// When the source value is <c>null</c>, no expression is generated for that member.
/// </remarks>
/// <example>
/// <code>
/// [StringFilter(TargetProperty = "Name", Order = 1)]
/// public string? Name { get; init; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class StringFilterAttribute : Attribute
{
    /// <summary>
    /// The target property path on the entity (supports dotted paths like <c>"Customer.Name"</c>).
    /// If not set, the DTO property name is used.
    /// </summary>
    public string? TargetProperty { get; set; }

    /// <summary>
    /// Ordering index for composing the final predicate. If not specified in usage,
    /// the generator falls back to the DTO declaration order.
    /// </summary>
    public int Order { get; set; }
}
