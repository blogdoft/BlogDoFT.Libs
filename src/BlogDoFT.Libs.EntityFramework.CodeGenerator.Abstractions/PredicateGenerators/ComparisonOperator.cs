namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

/// <summary>
/// Comparison operators supported by numeric and temporal filters.
/// </summary>
/// <remarks>
/// These operators are translated into the corresponding expression tree nodes.
/// </remarks>
public enum ComparisonOperator
{
    /// <summary>
    /// Equality comparison (<c>==</c>).
    /// </summary>
    Equal,

    /// <summary>
    /// Greater-than comparison (<c>&gt;</c>).
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Less-than comparison (<c>&lt;</c>).
    /// </summary>
    LessThan,

    /// <summary>
    /// Greater-than-or-equal comparison (<c>&gt;=</c>).
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Less-than-or-equal comparison (<c>&lt;=</c>).
    /// </summary>
    LessThanOrEqual
}
