using System.Data;

namespace BlogDoFT.Libs.DapperUtils.Abstractions;

public interface IDatabaseFacade : IDisposable
{
    IDbConnection GetDbConnection();

    Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null);

    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null);

    Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null);

    Task<TReturn?> ExecuteScalarAsync<TReturn>(string sql, object? param = null, IDbTransaction? transaction = null);

    Task<IGridReaderFacade> QueryMultipleAsync(
        string sql,
        object? param);
}
