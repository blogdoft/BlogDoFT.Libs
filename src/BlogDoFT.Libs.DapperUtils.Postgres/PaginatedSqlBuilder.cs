using BlogDoFT.Libs.DapperUtils.Abstractions;
using BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;
using System.Collections.Immutable;
using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

public class PaginatedSqlBuilder
{
    private readonly WhereBuilder _where;
    private string? _resultSet;
    private PageFilter _pageFilter;
    private ImmutableDictionary<string, string>? _orderMap;

    public PaginatedSqlBuilder()
    {
        _where = new WhereBuilder();
    }

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

    public PaginatedSqlBuilder WithResultSet(string resultSet)
    {
        _resultSet = resultSet;
        return this;
    }

    public PaginatedSqlBuilder MappingOrderWith(string key, string value) =>
        MappingOrderWith(new Dictionary<string, string> { { key, value } });

    public PaginatedSqlBuilder MappingOrderWith(Dictionary<string, string> orderMap) =>
        MappingOrderWith(orderMap.ToImmutableDictionary());

    public PaginatedSqlBuilder MappingOrderWith(ImmutableDictionary<string, string> orderMap)
    {
        _orderMap = orderMap;
        return this;
    }

    public PaginatedSqlBuilder WithPagination(PageFilter pageFilter)
    {
        _pageFilter = pageFilter;
        return this;
    }

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
