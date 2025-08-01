namespace BlogDoFT.Libs.DapperUtils.Abstractions;

public readonly struct PageFilter
{
    public int Page { get; init; }

    public int Size { get; init; }

    public string? Order { get; init; }
}
