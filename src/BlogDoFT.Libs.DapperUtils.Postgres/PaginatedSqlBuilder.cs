using BlogDoFT.Libs.DapperUtils.Abstractions;
using BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;
using System.Collections.Immutable;
using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

/// <summary>
/// Fluent builder that assembles a paginated SQL query and its matching row-count query, combining a result
/// set, a <see cref="WhereBuilder"/>, an <see cref="OrderByResolver"/> mapping, and a <see cref="PageFilter"/>.
/// </summary>
public class PaginatedSqlBuilder
{
    private readonly WhereBuilder _where;
    private string? _resultSet;
    private PageFilter _pageFilter;
    private ImmutableDictionary<string, string>? _orderMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedSqlBuilder"/> class.
    /// </summary>
    public PaginatedSqlBuilder()
    {
        _where = new WhereBuilder();
    }

    /// <summary>
    /// Builds the paginated query and its corresponding row-count query, from the result set, filters,
    /// ordering, and pagination configured on this builder.
    /// </summary>
    /// <returns>
    /// A tuple containing the paginated <c>Query</c> (with <c>WHERE</c>, <c>ORDER BY</c>, and <c>LIMIT</c>/<c>OFFSET</c>
    /// applied) and the <c>QuerySize</c> statement that counts the total rows matching the same filters.
    /// </returns>
    /// <exception cref="PaginatedSqlBuilderException">
    /// Thrown when no result set was configured via <see cref="WithResultSet"/>, or no pagination was
    /// configured via <see cref="WithPagination"/>.
    /// </exception>
    public (StringBuilder Query, StringBuilder QuerySize) Build()
    {
        ValidateBuild();

        var resultSet = new StringBuilder(_resultSet + Environment.NewLine);

        var where = _where.Build();

        var orderBy = new OrderByResolver(_orderMap)
            .Resolve(_pageFilter.Order);

        var paging = SqlPagination.From(_pageFilter);

        resultSet
            .Append(where);

        var querySize = PageCountStmt.BuildCountSql(resultSet);
        var paginatedQuery = resultSet
            .AppendLine()
            .AppendLine(orderBy.ToString())
            .AppendLine(paging);

        return (Query: paginatedQuery, QuerySize: querySize);
    }

    /// <summary>
    /// Sets the base SQL result set (e.g. a <c>SELECT ... FROM ...</c> statement) that filters, ordering, and
    /// pagination are applied on top of.
    /// </summary>
    /// <param name="resultSet">The base SQL query.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
    public PaginatedSqlBuilder WithResultSet(string resultSet)
    {
        _resultSet = resultSet;
        return this;
    }

    /// <summary>
    /// Adds a single user-facing field to SQL field mapping, used to resolve the <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="key">The user-facing field name.</param>
    /// <param name="value">The corresponding SQL field name.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
    public PaginatedSqlBuilder MappingOrderWith(string key, string value) =>
        MappingOrderWith(new Dictionary<string, string> { { key, value } });

    /// <summary>
    /// Sets the user-facing field to SQL field mapping, used to resolve the <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderMap">The mapping from user-facing field names to their corresponding SQL field names.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
    public PaginatedSqlBuilder MappingOrderWith(Dictionary<string, string> orderMap) =>
        MappingOrderWith(orderMap.ToImmutableDictionary());

    /// <summary>
    /// Sets the user-facing field to SQL field mapping, used to resolve the <c>ORDER BY</c> clause.
    /// </summary>
    /// <param name="orderMap">The mapping from user-facing field names to their corresponding SQL field names.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
    public PaginatedSqlBuilder MappingOrderWith(ImmutableDictionary<string, string> orderMap)
    {
        _orderMap = orderMap;
        return this;
    }

    /// <summary>
    /// Sets the page number and page size to apply to the query.
    /// </summary>
    /// <param name="pageFilter">The pagination settings.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
    public PaginatedSqlBuilder WithPagination(PageFilter pageFilter)
    {
        _pageFilter = pageFilter;
        return this;
    }

    /// <summary>
    /// Configures the <c>WHERE</c> clause conditions by invoking the given callback against the builder's
    /// internal <see cref="WhereBuilder"/>.
    /// </summary>
    /// <param name="where">A callback that adds conditions to the supplied <see cref="WhereBuilder"/>.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
    public PaginatedSqlBuilder WithWhere(Action<WhereBuilder> where)
    {
        where(_where);
        return this;
    }

    private void ValidateBuild()
    {
        if (string.IsNullOrWhiteSpace(_resultSet))
        {
            throw new PaginatedSqlBuilderException();
        }

        if (_pageFilter.Equals((PageFilter)default))
        {
            throw new PaginatedSqlBuilderException();
        }
    }
}
