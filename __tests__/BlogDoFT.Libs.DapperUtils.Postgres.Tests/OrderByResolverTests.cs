namespace BlogDoFT.Libs.DapperUtils.Postgres.Tests;

public class OrderByResolverTests
{
    [Fact]
    public void Should_ReturnEmpty_When_UserOrderByIsNull()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>());

        var result = resolver.Resolve(null);

        result.ToString().ShouldBeEmpty();
    }

    [Fact]
    public void Should_ReturnEmpty_When_UserOrderByIsEmpty()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>());

        var result = resolver.Resolve(string.Empty);

        result.ToString().ShouldBeEmpty();
    }

    [Fact]
    public void Should_ResolveSingleFieldAscending_When_ValidMappingExists()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>
        {
            { "name", "u.name" },
        });

        var result = resolver.Resolve("name");

        result.ToString().ShouldBe(
            $"ORDER BY{Environment.NewLine}u.name ASC{Environment.NewLine}");
    }

    [Fact]
    public void Should_ResolveSingleFieldDescending_When_DirectionSpecified()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>
        {
            { "name", "u.name" },
        });

        var result = resolver.Resolve("name desc");

        result.ToString().ShouldBe(
            $"ORDER BY{Environment.NewLine}u.name DESC{Environment.NewLine}");
    }

    [Fact]
    public void Should_SkipUnmappedFields_And_ReturnOnlyMappedOnes()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>
        {
            { "id", "u.id" },
        });

        var result = resolver.Resolve("id, unknown");

        result.ToString().ShouldBe(
            $"ORDER BY{Environment.NewLine}u.id ASC{Environment.NewLine}");
    }

    [Fact]
    public void Should_ResolveMultipleFields_WithMixedDirections()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>
        {
            { "id", "u.id" },
            { "name", "u.name" },
        });

        var result = resolver.Resolve("id DESC, name asc");

        result.ToString().ShouldBe(
            $"ORDER BY{Environment.NewLine}u.id DESC, u.name ASC{Environment.NewLine}");
    }

    [Fact]
    public void Should_HandleExtraSpaces_And_NormalizeInput()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>
        {
            { "created", "u.created_at" },
        });

        var result = resolver.Resolve("  created   desc  ");

        result.ToString().ShouldBe(
            $"ORDER BY{Environment.NewLine}u.created_at DESC{Environment.NewLine}");
    }

    [Fact]
    public void Should_ReturnEmpty_When_AllFieldsAreUnmapped()
    {
        var resolver = new OrderByResolver(new Dictionary<string, string>());

        var result = resolver.Resolve("invalid1, invalid2");

        result.ToString().ShouldBeEmpty();
    }
}
