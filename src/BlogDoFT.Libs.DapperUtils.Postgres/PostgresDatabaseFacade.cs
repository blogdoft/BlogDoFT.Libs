using BlogDoFT.Libs.DapperUtils.Abstractions;
using BlogDoFT.Libs.DapperUtils.Abstractions.Impl;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

[ExcludeFromCodeCoverage]
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
internal class PostgresDatabaseFacade : IDatabaseFacade
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
{
    private readonly IDbConnection _connection;

    public PostgresDatabaseFacade(IConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.GetNewConnection();
    }

    public void Dispose() =>
        _connection.Dispose();

    public async Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null)
    {
        await EnsureOpenAsync();
        return await _connection.QueryAsync<T>(sql, param);
    }

    public async Task<T> QueryFirstAsync<T>(
        string sql,
        object? param = null)
    {
        await EnsureOpenAsync();
        return await _connection.QueryFirstAsync<T>(sql, param);
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteAsync(sql, param, transaction);
    }

    public async Task<TReturn?> ExecuteScalarAsync<TReturn>(string sql, object? param = null, IDbTransaction? transaction = null)
    {
        await EnsureOpenAsync();
        return await _connection.ExecuteScalarAsync<TReturn>(sql, param, transaction);
    }

    public async Task<IGridReaderFacade> QueryMultipleAsync(string sql, object? param)
    {
        await EnsureOpenAsync();
        var multiple = await _connection.QueryMultipleAsync(sql, param);
        return new GridReaderFacade(multiple);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
    {
        await EnsureOpenAsync();
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, param);
    }

    public IDbConnection GetDbConnection()
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        return _connection;
    }

    private async Task EnsureOpenAsync(CancellationToken cancellationToken = default)
    {
        if (_connection.State == ConnectionState.Open)
        {
            return;
        }

        if (_connection is DbConnection adoConnection)
        {
            await adoConnection.OpenAsync(cancellationToken);
            return;
        }

        _connection.Open();
    }
}
