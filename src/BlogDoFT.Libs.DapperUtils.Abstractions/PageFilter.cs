namespace BlogDoFT.Libs.DapperUtils.Abstractions;

/// <summary>
/// Represents pagination and ordering criteria for a query.
/// </summary>
public readonly struct PageFilter
{
    /// <summary>
    /// Gets the page number to retrieve.
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    public int Size { get; init; }

    /// <summary>
    /// Gets the ordering clause to apply, if any.
    /// </summary>
    public string? Order { get; init; }
}
