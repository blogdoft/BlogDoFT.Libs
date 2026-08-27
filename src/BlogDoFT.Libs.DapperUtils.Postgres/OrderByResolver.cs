using BlogDoFT.Libs.Extensions;
using System.Collections.Immutable;
using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

/// <summary>
/// Translates user-supplied "order by" field names into their corresponding SQL <c>ORDER BY</c> clause,
/// using a mapping between the two.
/// </summary>
public class OrderByResolver
{
    private const string Ascending = "ASC";
    private const string Descending = "DESC";

    private readonly ImmutableDictionary<string, string> _fromTo;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByResolver"/> class.
    /// </summary>
    /// <param name="fromTo">A mapping from user-facing field names to their corresponding SQL field names.</param>
    public OrderByResolver(Dictionary<string, string> fromTo)
        : this(fromTo.ToImmutableDictionary())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderByResolver"/> class.
    /// </summary>
    /// <param name="fromTo">
    /// A mapping from user-facing field names to their corresponding SQL field names. When <see langword="null"/>,
    /// no field is resolved and <see cref="Resolve"/> always returns an empty <see cref="StringBuilder"/>.
    /// </param>
    public OrderByResolver(ImmutableDictionary<string, string>? fromTo) =>
        _fromTo = fromTo ?? new Dictionary<string, string>().ToImmutableDictionary();

    /// <summary>
    /// Builds a SQL <c>ORDER BY</c> clause from a comma-separated list of user-facing field names, each optionally
    /// followed by a space and an <c>ASC</c> or <c>DESC</c> direction (defaulting to <c>ASC</c>). Fields not present
    /// in the mapping supplied to the constructor are ignored.
    /// </summary>
    /// <param name="userOrderBy">The comma-separated, user-facing "order by" expression, e.g. <c>"name desc, id"</c>.</param>
    /// <returns>
    /// A <see cref="StringBuilder"/> containing the resolved <c>ORDER BY</c> clause, or an empty one when
    /// <paramref name="userOrderBy"/> is <see langword="null"/>, blank, or maps to no known field.
    /// </returns>
    public StringBuilder Resolve(string? userOrderBy)
    {
        var orderBy = new StringBuilder();
        if (string.IsNullOrWhiteSpace(userOrderBy))
        {
            return orderBy;
        }

        var fieldsList = MapUserToSqlFields(userOrderBy);
        if (!fieldsList.Any())
        {
            return orderBy;
        }

        var orderFields = string.Join(", ", fieldsList);

        return orderBy
            .AppendLine("ORDER BY")
            .AppendLine(orderFields);
    }

    private static IEnumerable<(string FieldName, string Ordering)> NormalizeInput(
        string userOrderBy) => userOrderBy
        .Split(",")
        .Select(field =>
        {
            var fieldMeta = field.Trim()
                .ReplaceAll("  ", " ")
                .Split(' ');
            if (fieldMeta.Length == 2)
            {
                var ordering = fieldMeta[1].ToUpper() == Descending
                    ? fieldMeta[1].ToUpper()
                    : Ascending;
                return (FieldName: fieldMeta[0], Ordering: ordering);
            }

            return (FieldName: fieldMeta[0], Ordering: Ascending);
        });

    private IEnumerable<string> MapUserToSqlFields(string userOrderBy)
    {
        if (string.IsNullOrWhiteSpace(userOrderBy))
        {
            return Array.Empty<string>();
        }

        var fields = NormalizeInput(userOrderBy);

        return MapFields(fields);
    }

    private IEnumerable<string> MapFields(
        IEnumerable<(string FieldName, string Ordering)> fieldsMeta) => fieldsMeta
        .Aggregate(new List<string>(), (orders, fieldMeta) =>
        {
            var mappedField = _fromTo.TryGetValue(fieldMeta.FieldName, out var sqlFieldName);

            if (string.IsNullOrWhiteSpace(sqlFieldName))
            {
                return orders;
            }

            if (mappedField)
            {
                orders.Add($"{sqlFieldName} {fieldMeta.Ordering}");
            }

            return orders;
        });
}
