using BlogDoFT.Libs.Extensions;
using System.Collections.Immutable;
using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

public class OrderByResolver
{
    private const string Ascending = "ASC";
    private const string Descending = "DESC";

    private readonly ImmutableDictionary<string, string> _fromTo;

    public OrderByResolver(Dictionary<string, string> fromTo)
        : this(fromTo.ToImmutableDictionary())
    {
    }

    public OrderByResolver(ImmutableDictionary<string, string>? fromTo) =>
        _fromTo = fromTo ?? new Dictionary<string, string>().ToImmutableDictionary();

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
