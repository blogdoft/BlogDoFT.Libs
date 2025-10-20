#pragma warning disable S2326
namespace BlogDoFT.Libs.EntityFramework.CodeGenerator.Abstractions.PredicateGenerators;

/// <summary>
/// Marks a filter DTO to receive generated members such as <c>ToPredicate()</c> and <c>HasFilter()</c>.
/// Use the generic type parameter to indicate the target entity type.
/// </summary>
/// <typeparam name="TEntity">
/// The entity type of the <c>DbSet&lt;TEntity&gt;</c> to which the generated predicate will apply.
/// </typeparam>
/// <example>
/// <code>
/// [GeneratePredicate&lt;UserRecord&gt;]
/// public partial class UserFilterDto { }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class)]
public sealed class GeneratePredicateAttribute<TEntity> : Attribute
{
}
#pragma warning restore S2326
