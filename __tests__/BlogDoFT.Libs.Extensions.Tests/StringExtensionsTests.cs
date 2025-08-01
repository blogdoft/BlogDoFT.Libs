namespace BlogDoFT.Libs.Extensions.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void Should_ReturnOriginalString_When_OldStringDoesNotExist()
    {
        const string Input = "Hello World";

        var result = Input.ReplaceAll("X", "Y");

        result.ShouldBe("Hello World");
    }

    [Fact]
    public void Should_ReplaceSingleOccurrence_When_OldStringAppearsOnce()
    {
        var result = "Hello Foo".ReplaceAll("Foo", "Bar");

        result.ShouldBe("Hello Bar");
    }

    [Fact]
    public void Should_ReplaceAllOccurrences_When_OldStringAppearsMultipleTimes()
    {
        var result = "foo foo foo".ReplaceAll("foo", "bar");

        result.ShouldBe("bar bar bar");
    }

    [Fact]
    public void Should_ThrowArgumentException_When_OldStringIsEmpty()
    {
        Should.Throw<ArgumentException>(() => "abc".ReplaceAll("", "x"));
    }

    [Fact]
    public void Should_ReturnSameString_When_OldStringEqualsNewString()
    {
        var result = "abcabc".ReplaceAll("abc", "abc");

        result.ShouldBe("abcabc");
    }
}
