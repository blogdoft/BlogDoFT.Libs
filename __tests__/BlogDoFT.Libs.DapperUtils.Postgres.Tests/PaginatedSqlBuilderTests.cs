using BlogDoFT.Libs.DapperUtils.Abstractions;
using BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class PaginatedSqlBuilderTests
{
    [Fact]
    public void Should_ThrowException_When_ResultSetIsMissing()
    {
        var builder = new PaginatedSqlBuilder()
            .WithPagination(new PageFilter { Page = 1, Size = 10 });

        Should.Throw<PaginatedSqlBuilderException>(() => builder.Build());
    }

    [Fact]
    public void Should_ThrowException_When_PaginationIsMissing()
    {
        var builder = new PaginatedSqlBuilder()
            .WithResultSet("select * from table");

        Should.Throw<PaginatedSqlBuilderException>(() => builder.Build());
    }

    [Fact]
    public void Should_GenerateValidSql_When_MinimalDataProvided()
    {
        var page = new PageFilter { Page = 0, Size = 10 };

        var builder = new PaginatedSqlBuilder()
            .WithResultSet("select * from users")
            .WithPagination(page);

        var (query, countQuery) = builder.Build();

        query.ToString().ShouldContain("select * from users");
        query.ToString().ShouldContain("LIMIT 10 OFFSET 0");
        countQuery.ToString().ShouldContain("select count(1) from (select * from users");
    }

    [Fact]
    public void Should_IncludeOrderByClause_When_MappingProvided()
    {
        var page = new PageFilter { Page = 1, Size = 5, Order = "name DESC" };

        var builder = new PaginatedSqlBuilder()
            .WithResultSet("select * from users")
            .WithPagination(page)
            .MappingOrderWith("name", "u.name");

        var (query, _) = builder.Build();

        query.ToString().ShouldContain("ORDER BY");
        query.ToString().ShouldContain("u.name DESC");
        query.ToString().ShouldContain("LIMIT 5 OFFSET 5");
    }

    [Fact]
    public void Should_IncludeWhereClause_WithAnd_When_ConditionIsMet()
    {
        var page = new PageFilter { Page = 1, Size = 10 };

        var builder = new PaginatedSqlBuilder()
            .WithResultSet("select * from products")
            .WithPagination(page)
            .WithWhere(w => w
                .AndWith(true, "active = true")
                .AndWith(true, "active = true"));

        var (query, _) = builder.Build();

        query.ToString().ShouldContain("where  (active = true)", Case.Insensitive);
        query.ToString().ShouldContain("and (active = true)", Case.Insensitive);
    }

    [Fact]
    public void Should_IncludeWhereClause_WithOr_When_ConditionIsMet()
    {
        var page = new PageFilter { Page = 1, Size = 10 };

        var builder = new PaginatedSqlBuilder()
            .WithResultSet("select * from products")
            .WithPagination(page)
            .WithWhere(w => w
                .OrWith(true, "discontinued = false")
                .OrWith(true, "discontinued = false"));

        var (query, _) = builder.Build();

        query.ToString().ShouldContain("where  (discontinued = false)", Case.Insensitive);
        query.ToString().ShouldContain("or (discontinued = false)", Case.Insensitive);
    }

    [Fact]
    public void Should_NotIncludeWhereClause_When_ValueIsNull()
    {
        var page = new PageFilter { Page = 1, Size = 10 };

        var builder = new PaginatedSqlBuilder()
            .WithResultSet("select * from products")
            .WithPagination(page)
            .WithWhere(w => w.AndWith(null, "active = true"));

        var (query, _) = builder.Build();

        query.ToString().ShouldNotContain("where", Case.Insensitive);
    }
}
