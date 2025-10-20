using System.Text;

namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class PageCountStmtTests
{
    [Fact]
    public void Should_BuildCountSql_FromRawQuery()
    {
        // Given
        var baseSql = new StringBuilder("select * from users where active = true");

        // When
        var result = PageCountStmt.BuildCountSql(baseSql);

        // Then
        result.ToString().ShouldBe("select count(1) from (select * from users where active = true) as counter_result");
    }

    [Fact]
    public void Should_AllowSqlWithLineBreaks()
    {
        // Given
        var baseSql = new StringBuilder("select * \nfrom products\nwhere stock > 0");

        // When
        var result = PageCountStmt.BuildCountSql(baseSql);

        // Then
        result.ToString().ShouldBe("select count(1) from (select * \nfrom products\nwhere stock > 0) as counter_result");
    }

    [Fact]
    public void Should_ReturnFormattedSql_WhenEmptyInput()
    {
        // Given
        var baseSql = new StringBuilder();

        // When
        var result = PageCountStmt.BuildCountSql(baseSql);

        // Then
        result.ToString().ShouldBe("select count(1) from () as counter_result");
    }

    [Fact]
    public void Should_NotModifyOriginalInput()
    {
        // Given
        var baseSql = new StringBuilder("select * from test");

        // When
        _ = PageCountStmt.BuildCountSql(baseSql);

        // Then
        baseSql.ToString().ShouldBe("select * from test"); // input deve permanecer inalterado
    }
}
