using System.Diagnostics.CodeAnalysis;
using static Dapper.SqlMapper;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.Impl;

/// <summary>
/// Default <see cref="IGridReaderFacade"/> implementation that wraps a Dapper <see cref="GridReader"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class GridReaderFacade : IGridReaderFacade
{
    private readonly GridReader _gridReader;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GridReaderFacade"/> class.
    /// </summary>
    /// <param name="gridReader">The Dapper grid reader to wrap.</param>
    public GridReaderFacade(GridReader gridReader) =>
        _gridReader = gridReader;

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public IEnumerable<T> Read<T>(bool buffered = true) =>
        _gridReader.Read<T>(buffered);

    /// <inheritdoc/>
    public Task<IEnumerable<T>> ReadAsync<T>(bool buffered = true)
        => _gridReader.ReadAsync<T>(buffered);

    /// <inheritdoc/>
    public Task<T?> ReadFirstOrDefaultAsync<T>() =>
        _gridReader.ReadFirstOrDefaultAsync<T>();

    /// <summary>
    /// Releases the resources used by the underlying grid reader.
    /// </summary>
    /// <param name="disposing">When <see langword="true"/>, releases both managed and unmanaged resources; otherwise releases only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _gridReader.Dispose();
        }

        _disposed = true;
    }
}
