using Bogus;
using NSubstitute;

namespace BlogDoFT.Libs.ResultPattern.Tests.ResultExtensionTests;

public sealed class MapExtensionTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Should_ReturnSuccessProjection_When_ResultIsSuccess()
    {
        // Given
        var successValue = _faker.Random.String2(10);
        Result<string> result = successValue;
        var expectedValue = _faker.Random.Int(1, 1000);

        var onSuccess = Substitute.For<Func<string, int>>();
        onSuccess.Invoke(successValue).Returns(expectedValue);
        var onFailure = Substitute.For<Func<Failure, int>>();

        // When
        var actualValue = result.Map(onSuccess, onFailure);

        // Then
        actualValue.ShouldBe(expectedValue);
        onSuccess.Received(1).Invoke(successValue);
        onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public void Should_ReturnFailureProjection_When_ResultIsFailure()
    {
        // Given
        var failure = new Failure(_faker.Random.String2(5), _faker.Lorem.Sentence());
        Result<string> result = failure;
        var expectedValue = _faker.Random.Int();

        var onSuccess = Substitute.For<Func<string, int>>();
        var onFailure = Substitute.For<Func<Failure, int>>();
        onFailure.Invoke(failure).Returns(expectedValue);

        // When
        var actualValue = result.Map(onSuccess, onFailure);

        // Then
        actualValue.ShouldBe(expectedValue);
        onFailure.Received(1).Invoke(failure);
        onSuccess.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task Should_ReturnSuccessProjection_When_TaskResultIsSuccessAsync()
    {
        // Given
        var successValue = _faker.Random.String2(8);
        var resultTask = Task.FromResult<Result<string>>(successValue);
        var expectedValue = _faker.Lorem.Word();

        var onSuccess = Substitute.For<Func<string, string>>();
        onSuccess.Invoke(successValue).Returns(expectedValue);
        var onFailure = Substitute.For<Func<Failure, string>>();

        // When
        var actualValue = await resultTask.MapAsync(onSuccess, onFailure);

        // Then
        actualValue.ShouldBe(expectedValue);
        onSuccess.Received(1).Invoke(successValue);
        onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task Should_ReturnFailureProjection_When_TaskResultIsFailureAsync()
    {
        // Given
        var failure = new Failure(_faker.Random.String2(6), _faker.Lorem.Sentence());
        var resultTask = Task.FromResult<Result<string>>(failure);
        var expectedValue = _faker.Lorem.Word();

        var onSuccess = Substitute.For<Func<string, string>>();
        var onFailure = Substitute.For<Func<Failure, string>>();
        onFailure.Invoke(failure).Returns(expectedValue);

        // When
        var actualValue = await resultTask.MapAsync(onSuccess, onFailure);

        // Then
        actualValue.ShouldBe(expectedValue);
        onFailure.Received(1).Invoke(failure);
        onSuccess.DidNotReceiveWithAnyArgs().Invoke(default!);
    }
}
