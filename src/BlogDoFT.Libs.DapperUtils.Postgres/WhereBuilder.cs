using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

public class WhereBuilder
{
    private const string OpenAnd = " and (";

    // extra empty space needed to not broken when
    // trim the first for characters
    private const string OpenOr = "  or (";
    private readonly StringBuilder _where;

    public WhereBuilder() =>
        _where = new StringBuilder();

    public StringBuilder Build()
    {
        if (_where.Length == 0)
        {
            return new StringBuilder();
        }

        return new StringBuilder("where ")
            .Append(_where.ToString()[4..]);
    }

    public WhereBuilder AndWith(object? paramValue, string condition)
    {
        if (paramValue is not null)
        {
            _where
                .Append(OpenAnd)
                .Append(condition)
                .AppendLine(") ");
        }

        return this;
    }

    public WhereBuilder OrWith(object? paramValue, string condition)
    {
        if (paramValue is not null)
        {
            _where
                .Append(OpenOr)
                .Append(condition)
                .AppendLine(") ");
        }

        return this;
    }
}
