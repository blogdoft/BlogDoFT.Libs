using Dapper;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.TypeHandlers;

/// <summary>
/// Dapper type handler that maps <see cref="DateOnly"/> values to and from <see cref="DateTime"/> database columns.
/// </summary>
[ExcludeFromCodeCoverage]
public class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    /// <inheritdoc/>
    public override void SetValue(IDbDataParameter parameter, DateOnly date)
        => parameter.Value = date.ToDateTime(new TimeOnly(0, 0));

    /// <inheritdoc/>
    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);
}
