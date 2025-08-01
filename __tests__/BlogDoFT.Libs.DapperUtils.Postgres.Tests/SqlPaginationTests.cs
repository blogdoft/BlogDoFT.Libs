using BlogDoFT.Libs.DapperUtils.Abstractions;

namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class SqlPaginationTests
{
    [Fact]
    public void Should_ReturnCorrectPagination_When_ValidInput()
    {
        var result = SqlPagination.GetPagination(2, 10);

        result.ShouldBe("LIMIT 10 OFFSET 20");
    }

    [Fact]
    public void Should_ReturnCorrectPagination_When_PageIsZero()
    {
        var result = SqlPagination.GetPagination(0, 50);

        result.ShouldBe("LIMIT 50 OFFSET 0");
    }

    [Fact]
    public void Should_ThrowException_When_PageIsNegative()
    {
        var ex = Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            SqlPagination.GetPagination(-1, 10);
        });

        ex.Message.ShouldContain("Page number must be between 0 and");
    }

    [Fact]
    public void Should_ThrowException_When_SizeIsLessThanMin()
    {
        var ex = Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            SqlPagination.GetPagination(0, 0);
        });

        ex.Message.ShouldContain("Page size must be between 1 and");
    }

    [Fact]
    public void Should_ThrowException_When_SizeIsGreaterThanMax()
    {
        var ex = Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            SqlPagination.GetPagination(0, 1000);
        });

        ex.Message.ShouldContain("Page size must be between 1 and");
    }

    [Fact]
    public void Should_UseFromMethod_When_PageFilterProvided()
    {
        var filter = new PageFilter { Page = 3, Size = 25 };

        var result = SqlPagination.From(filter);

        result.ShouldBe("LIMIT 25 OFFSET 75");
    }
}
