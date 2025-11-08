using Bogus;
using NSubstitute;

namespace BlogDoFT.Libs.ResultPattern.Tests.ResultExtensionTests;

public sealed class ResultExtensionOnAsyncTests
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task Should_ReturnSuccessDelegateValue_When_ResultTaskIsSuccessfulAsync()
    {
        // Given
        Task<Result<string>> resultTask = Task.FromResult<Result<string>>(_faker.Random.String2(10));
        var expectedValue = _faker.Random.String2(15);

        var onSuccess = Substitute.For<Func<string>>();
        onSuccess.Invoke().Returns(expectedValue);

        var onFailure = Substitute.For<Func<Failure, string>>();

        // When
        var actualValue = await resultTask.OnAsync(onSuccess, onFailure);

        // Then
        actualValue.ShouldBe(expectedValue);
        onSuccess.Received(1).Invoke();
        onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task Should_InvokeFailureDelegate_When_ResultTaskFailsAsync()
    {
        // Given
        var failure = new Failure(
            Code: _faker.Random.String2(5),
            Message: _faker.Random.String2(25));
        Task<Result<int>> resultTask = Task.FromResult<Result<int>>(failure);
        var expectedFallback = _faker.Random.Int(1, 1000);

        var onSuccess = Substitute.For<Func<int>>();
        var onFailure = Substitute.For<Func<Failure, int>>();
        onFailure.Invoke(failure).Returns(expectedFallback);

        // When
        var actualValue = await resultTask.OnAsync(onSuccess, onFailure);

        // Then
        actualValue.ShouldBe(expectedFallback);
        onSuccess.DidNotReceive().Invoke();
        onFailure.Received(1).Invoke(failure);
    }
}
