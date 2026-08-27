using System.Data;

namespace BlogDoFT.Libs.DapperUtils.Abstractions;

/// <summary>
/// Abstracts Dapper database operations to enable testing and decouple consumers from a concrete connection.
/// </summary>
public interface IDatabaseFacade : IDisposable
{
    /// <summary>
    /// Gets the underlying database connection.
    /// </summary>
    /// <returns>The <see cref="IDbConnection"/> used by this facade.</returns>
    IDbConnection GetDbConnection();

    /// <summary>
    /// Executes a query and returns the resulting rows mapped to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to map each result row to.</typeparam>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <param name="param">The parameters to pass to the query, if any.</param>
    /// <returns>The sequence of mapped results.</returns>
    Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null);

    /// <summary>
    /// Executes a query and returns the first result, or the default value of <typeparamref name="T"/> if no result is found.
    /// </summary>
    /// <typeparam name="T">The type to map the result row to.</typeparam>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <param name="param">The parameters to pass to the query, if any.</param>
    /// <returns>The first mapped result, or <see langword="default"/> if none was found.</returns>
    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null);

    /// <summary>
    /// Executes a non-query SQL command.
    /// </summary>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <param name="param">The parameters to pass to the command, if any.</param>
    /// <param name="transaction">The transaction to execute the command within, if any.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null);

    /// <summary>
    /// Executes a SQL statement and returns the first column of the first row of the result.
    /// </summary>
    /// <typeparam name="TReturn">The type to convert the scalar result to.</typeparam>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <param name="param">The parameters to pass to the command, if any.</param>
    /// <param name="transaction">The transaction to execute the command within, if any.</param>
    /// <returns>The scalar result, or the default value of <typeparamref name="TReturn"/> if the result is <see langword="null"/>.</returns>
    Task<TReturn?> ExecuteScalarAsync<TReturn>(string sql, object? param = null, IDbTransaction? transaction = null);

    /// <summary>
    /// Executes a SQL statement that returns multiple result sets.
    /// </summary>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <param name="param">The parameters to pass to the query.</param>
    /// <returns>An <see cref="IGridReaderFacade"/> used to read the multiple result sets.</returns>
    Task<IGridReaderFacade> QueryMultipleAsync(
        string sql,
        object? param);
}
