using Dapper;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.TypeHandlers;

/// <summary>
/// Dapper type handler that maps <see cref="TimeOnly"/> values to and from database columns.
/// </summary>
[ExcludeFromCodeCoverage]
public class SqlTimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    /// <inheritdoc/>
    public override void SetValue(IDbDataParameter parameter, TimeOnly time)
    {
        parameter.Value = time.ToString();
    }

    /// <inheritdoc/>
    public override TimeOnly Parse(object value) => TimeOnly.FromTimeSpan((TimeSpan)value);
}
