using BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;
using Shouldly;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.Tests.Extensions.SqlExtensionsTests;

public class ToSearchableTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ReturnEmpty_When_InputIsNullOrWhitespace(string? input)
    {
        input.ToSearchable().ShouldBe(string.Empty);
    }

    [Fact]
    public void Should_RemoveAccentsAndUppercase()
    {
        const string Input = "Olá, João!";
        const string Expected = "OLA, JOAO!";
        Input.ToSearchable().ShouldBe(Expected);
    }

    [Fact]
    public void Should_ReplaceCedillaLowercase()
    {
        const string Input = "coração";
        const string Expected = "CORACAO";
        Input.ToSearchable().ShouldBe(Expected);
    }

    [Fact]
    public void Should_ReplaceCedillaUppercase()
    {
        const string Input = "AÇÃO COM Ç";
        const string Expected = "ACAO COM C";
        Input.ToSearchable().ShouldBe(Expected);
    }

    [Fact]
    public void Should_HandleMixedCharacters()
    {
        const string Input = "Münchën ç Ç";
        const string Expected = "MUNCHEN C C";
        Input.ToSearchable().ShouldBe(Expected);
    }

    [Fact]
    public void Should_PreserveNonAccentedCharacters()
    {
        const string Input = "ABC123";
        const string Expected = "ABC123";
        Input.ToSearchable().ShouldBe(Expected);
    }

    [Fact]
    public void Should_ConvertToUppercase()
    {
        const string Input = "letra minúscula";
        const string Expected = "LETRA MINUSCULA";
        Input.ToSearchable().ShouldBe(Expected);
    }
}
