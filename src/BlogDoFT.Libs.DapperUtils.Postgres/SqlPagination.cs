using BlogDoFT.Libs.DapperUtils.Abstractions;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

public static class SqlPagination
{
    internal const int MinPageNumber = 0;
    internal const int MinPageSize = 1;
    internal const int MaxPageSize = 255;

    private static readonly string _pageNumberRangeError =
        $"Page number must be between 0 and {int.MaxValue}";

    private static readonly string _pageSizeRangeError =
        $"Page size must be between {MinPageSize} and {MaxPageSize}";

    public static string GetPagination(int pageNumber, int pageSize) =>
         From(new PageFilter { Page = pageNumber, Size = pageSize });

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
