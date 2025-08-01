using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class PageCountStmtTests
{
    [Fact]
    public void Should_BuildCountSql_FromRawQuery()
    {
        // Arrange
        var baseSql = new StringBuilder("select * from users where active = true");

        // Act
        var result = PageCountStmt.BuildCountSql(baseSql);

        // Assert
        result.ToString().ShouldBe("select count(1) from (select * from users where active = true) as counter_result");
    }

    [Fact]
    public void Should_AllowSqlWithLineBreaks()
    {
        // Arrange
        var baseSql = new StringBuilder("select * \nfrom products\nwhere stock > 0");

        // Act
        var result = PageCountStmt.BuildCountSql(baseSql);

        // Assert
        result.ToString().ShouldBe("select count(1) from (select * \nfrom products\nwhere stock > 0) as counter_result");
    }

    [Fact]
    public void Should_ReturnFormattedSql_WhenEmptyInput()
    {
        // Arrange
        var baseSql = new StringBuilder();

        // Act
        var result = PageCountStmt.BuildCountSql(baseSql);

        // Assert
        result.ToString().ShouldBe("select count(1) from () as counter_result");
    }

    [Fact]
    public void Should_NotModifyOriginalInput()
    {
        // Arrange
        var baseSql = new StringBuilder("select * from test");

        // Act
        var _ = PageCountStmt.BuildCountSql(baseSql);

        // Assert
        baseSql.ToString().ShouldBe("select * from test"); // input deve permanecer inalterado
    }
}
