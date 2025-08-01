using Dapper;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.TypeHandlers;

[ExcludeFromCodeCoverage]
public class SqlDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(IDbDataParameter parameter, DateOnly date)
        => parameter.Value = date.ToDateTime(new TimeOnly(0, 0));

    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);
}
