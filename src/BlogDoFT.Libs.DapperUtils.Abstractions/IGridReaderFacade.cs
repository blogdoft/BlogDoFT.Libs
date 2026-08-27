namespace BlogDoFT.Libs.DapperUtils.Abstractions;

/// <summary>
/// Abstracts Dapper's grid reader to allow reading multiple result sets produced by a single query.
/// </summary>
public interface IGridReaderFacade : IDisposable
{
    /// <summary>
    /// Reads the next result set and returns its first row, or the default value of <typeparamref name="T"/> if it is empty.
    /// </summary>
    /// <typeparam name="T">The type to map the result row to.</typeparam>
    /// <returns>The first mapped result, or <see langword="default"/> if none was found.</returns>
    Task<T?> ReadFirstOrDefaultAsync<T>();

    /// <summary>
    /// Reads the next result set synchronously.
    /// </summary>
    /// <typeparam name="T">The type to map each result row to.</typeparam>
    /// <param name="buffered">When <see langword="true"/>, the entire result set is read into memory before returning. Defaults to <see langword="true"/>.</param>
    /// <returns>The sequence of mapped results.</returns>
    IEnumerable<T> Read<T>(bool buffered = true);

    /// <summary>
    /// Reads the next result set asynchronously.
    /// </summary>
    /// <typeparam name="T">The type to map each result row to.</typeparam>
    /// <param name="buffered">When <see langword="true"/>, the entire result set is read into memory before returning. Defaults to <see langword="true"/>.</param>
    /// <returns>The sequence of mapped results.</returns>
    Task<IEnumerable<T>> ReadAsync<T>(bool buffered = true);
}
