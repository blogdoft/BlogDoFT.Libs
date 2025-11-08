using Bogus;
using NSubstitute;

namespace BlogDoFT.Libs.Extensions.Tests;

public sealed class EnumExtensionsTests
{
    private readonly Faker _faker = new();

    private enum SampleStatus
    {
        Unknown = 0,
        Pending = 1,
        Completed = 2,
    }

    [Fact]
    public void Should_ReturnUnderlyingInteger_When_AsIntegerIsCalled()
    {
        // Given
        var enumValue = _faker.PickRandom(Enum.GetValues<SampleStatus>());

        // When
        var actualValue = enumValue.AsInteger();

        // Then
        actualValue.ShouldBe((int)enumValue);
    }

    [Fact]
    public void Should_ParseEnum_When_StringRepresentsValidValue()
    {
        // Given
        var enumValue = _faker.PickRandom(Enum.GetValues<SampleStatus>());
        var provider = Substitute.For<Func<string>>();
        provider.Invoke().Returns(enumValue.ToString());

        // When
        var parsedValue = provider().ToEnum<SampleStatus>();

        // Then
        provider.Received(1).Invoke();
        parsedValue.ShouldBe(enumValue);
    }

    [Fact]
    public void Should_ThrowInvalidCastException_When_StringIsInvalid()
    {
        // Given
        var invalidValue = _faker.Random.AlphaNumeric(10);
        var provider = Substitute.For<Func<string>>();
        provider.Invoke().Returns(invalidValue);

        // When
        var exception = Should.Throw<InvalidCastException>(() => provider().ToEnum<SampleStatus>());

        // Then
        exception.Message.ShouldContain(invalidValue);
        provider.Received(1).Invoke();
    }
}
