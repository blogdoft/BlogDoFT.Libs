using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

/// <summary>
/// Fluent builder that assembles a SQL <c>WHERE</c> clause from conditions, skipping any whose associated
/// parameter value is <see langword="null"/>.
/// </summary>
public class WhereBuilder
{
    private const string OpenAnd = " and (";

    // extra empty space needed to not broken when
    // trim the first for characters
    private const string OpenOr = "  or (";
    private readonly StringBuilder _where;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhereBuilder"/> class.
    /// </summary>
    public WhereBuilder() =>
        _where = new StringBuilder();

    /// <summary>
    /// Builds the final SQL <c>WHERE</c> clause from the conditions added so far.
    /// </summary>
    /// <returns>
    /// A <see cref="StringBuilder"/> starting with <c>"where "</c> followed by the accumulated conditions,
    /// or an empty <see cref="StringBuilder"/> when no condition was added.
    /// </returns>
    public StringBuilder Build()
    {
        if (_where.Length == 0)
        {
            return new StringBuilder();
        }

        return new StringBuilder("where ")
            .Append(_where.ToString()[4..]);
    }

    /// <summary>
    /// Adds a condition combined with <c>AND</c> to the clause, but only when <paramref name="paramValue"/> is not <see langword="null"/>.
    /// </summary>
    /// <param name="paramValue">The parameter value associated with the condition. When <see langword="null"/>, the condition is skipped.</param>
    /// <param name="condition">The SQL condition text to add.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
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

    /// <summary>
    /// Adds a condition combined with <c>OR</c> to the clause, but only when <paramref name="paramValue"/> is not <see langword="null"/>.
    /// </summary>
    /// <param name="paramValue">The parameter value associated with the condition. When <see langword="null"/>, the condition is skipped.</param>
    /// <param name="condition">The SQL condition text to add.</param>
    /// <returns>The same builder instance, to allow chaining.</returns>
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
