using BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;
using Shouldly;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.Tests.Extensions;

public class SqlExtensionsTests
{
    [Fact]
    public void Should_ReplaceStartToPercent_When_TextContainsStar()
    {
        // Given
        const string Input = "My Text with * in middle";
        const string Expected = "MY TEXT WITH % IN MIDDLE";

        // When
        var transformedStr = Input.AsSqlWildCard();

        // Then
        transformedStr.ShouldBe(Expected);
    }

    [Fact]
    public void Should_JustUpperCase_When_ThereIsNoWildCard()
    {
        // Given
        const string Input = "There is no wild card";
        const string Expected = "THERE IS NO WILD CARD";

        // When
        var transformedStr = Input.AsSqlWildCard();

        // Then
        transformedStr.ShouldBe(Expected);
    }

    [Fact]
    public void Should_NotUpperCase_When_ProvideFalseToParameter()
    {
        // Given
        const string Input = "My Text with * in middle";
        const string Expected = "My Text with % in middle";

        // When
        var transformedStr = Input.AsSqlWildCard(toUpperCase: false);

        // Then
        transformedStr.ShouldBe(Expected);
    }

    [Fact]
    public void Should_NoChangeInput_When_ThereIsNoWildCardAndToUpperCaseIsFalse()
    {
        // Given
        const string Input = "There is no wild card";
        const string Expected = "There is no wild card";

        // When
        var transformedStr = Input.AsSqlWildCard(toUpperCase: false);

        // Then
        transformedStr.ShouldBe(Expected);
    }
}
