using BlogDoFT.Libs.DapperUtils.Abstractions;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

/// <summary>
/// Builds SQL <c>LIMIT</c>/<c>OFFSET</c> clauses for paginated queries.
/// </summary>
public static class SqlPagination
{
    internal const int MinPageNumber = 0;
    internal const int MinPageSize = 1;
    internal const int MaxPageSize = 255;

    private static readonly string _pageNumberRangeError =
        $"Page number must be between 0 and {int.MaxValue}";

    private static readonly string _pageSizeRangeError =
        $"Page size must be between {MinPageSize} and {MaxPageSize}";

    /// <summary>
    /// Builds a SQL <c>LIMIT</c>/<c>OFFSET</c> clause for the given page number and page size.
    /// </summary>
    /// <param name="pageNumber">The zero-based page number to retrieve.</param>
    /// <param name="pageSize">The number of rows per page.</param>
    /// <returns>The SQL <c>LIMIT</c>/<c>OFFSET</c> clause.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="pageNumber"/> is negative, or <paramref name="pageSize"/> is outside the allowed range.
    /// </exception>
    public static string GetPagination(int pageNumber, int pageSize) =>
         From(new PageFilter { Page = pageNumber, Size = pageSize });

    /// <summary>
    /// Builds a SQL <c>LIMIT</c>/<c>OFFSET</c> clause from the given <see cref="PageFilter"/>.
    /// </summary>
    /// <param name="pagination">The page and size to build the clause from.</param>
    /// <returns>The SQL <c>LIMIT</c>/<c>OFFSET</c> clause.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="pagination"/>'s page is negative, or its size is outside the allowed range.
    /// </exception>
    public static string From(PageFilter pagination)
    {
        if (pagination.Page < MinPageNumber)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pagination),
                _pageNumberRangeError);
        }

        if (pagination.Size < MinPageSize || pagination.Size > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pagination),
                _pageSizeRangeError);
        }

        return $"LIMIT {pagination.Size} OFFSET {pagination.Page * pagination.Size}";
    }
}
