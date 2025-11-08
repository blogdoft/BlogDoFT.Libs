using Bogus;
using NSubstitute;

namespace BlogDoFT.Libs.ResultPattern.Tests.ResultExtensionTests;

public sealed class TapFailureExtensionTests
{
    private readonly Faker _faker = new();

    [Fact]
    public void Should_InvokeFailureAction_When_ResultIsFailure()
    {
        // Given
        var failure = new Failure(_faker.Random.String2(5), _faker.Lorem.Sentence());
        Result<int> result = failure;
        var onFailure = Substitute.For<Action<Failure>>();

        // When
        var returnedResult = result.TapFailure(onFailure);

        // Then
        returnedResult.ShouldBeSameAs(result);
        onFailure.Received(1).Invoke(failure);
    }

    [Fact]
    public void Should_NotInvokeFailureAction_When_ResultIsSuccess()
    {
        // Given
        Result<int> result = _faker.Random.Int(1, 1000);
        var onFailure = Substitute.For<Action<Failure>>();

        // When
        var returnedResult = result.TapFailure(onFailure);

        // Then
        returnedResult.ShouldBeSameAs(result);
        onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task Should_InvokeAsyncFailureHandler_When_TaskResultFailsAsync()
    {
        // Given
        var failure = new Failure(_faker.Random.String2(6), _faker.Lorem.Sentence());
        var resultTask = Task.FromResult<Result<string>>(failure);
        var onFailureAsync = Substitute.For<Func<Failure, Task>>();
        onFailureAsync.Invoke(failure).Returns(Task.CompletedTask);

        // When
        var returnedResult = await resultTask.TapFailureAsync(onFailureAsync);

        // Then
        returnedResult.ShouldBeSameAs(await resultTask);
        await onFailureAsync.Received(1).Invoke(failure);
    }

    [Fact]
    public async Task Should_InvokeAsyncFailureHandler_When_ResultFailsAsync()
    {
        // Given
        var failure = new Failure(_faker.Random.String2(7), _faker.Lorem.Sentence());
        Result<string> result = failure;
        var onFailureAsync = Substitute.For<Func<Failure, Task>>();
        onFailureAsync.Invoke(failure).Returns(Task.CompletedTask);

        // When
        var returnedResult = await result.TapFailureAsync(onFailureAsync);

        // Then
        returnedResult.ShouldBeSameAs(result);
        await onFailureAsync.Received(1).Invoke(failure);
    }

    [Fact]
    public async Task Should_NotInvokeAsyncFailureHandler_When_ResultSucceedsAsync()
    {
        // Given
        Result<string> result = _faker.Random.String2(12);
        var onFailureAsync = Substitute.For<Func<Failure, Task>>();
        onFailureAsync.Invoke(Arg.Any<Failure>()).Returns(Task.CompletedTask);

        // When
        var returnedResult = await result.TapFailureAsync(onFailureAsync);

        // Then
        returnedResult.ShouldBeSameAs(result);
        await onFailureAsync.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task Should_InvokeSyncFailureAction_When_TaskResultFailsAsync()
    {
        // Given
        var failure = new Failure(_faker.Random.String2(5), _faker.Lorem.Sentence());
        var resultTask = Task.FromResult<Result<Guid>>(failure);
        var onFailure = Substitute.For<Action<Failure>>();

        // When
        var returnedResult = await resultTask.TapFailureAsync(onFailure);

        // Then
        returnedResult.ShouldBeSameAs(await resultTask);
        onFailure.Received(1).Invoke(failure);
    }

    [Fact]
    public async Task Should_NotInvokeSyncFailureAction_When_TaskResultSucceedsAsync()
    {
        // Given
        var value = Guid.NewGuid();
        var resultTask = Task.FromResult<Result<Guid>>(value);
        var onFailure = Substitute.For<Action<Failure>>();

        // When
        var returnedResult = await resultTask.TapFailureAsync(onFailure);

        // Then
        returnedResult.ShouldBeSameAs(await resultTask);
        onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }
}
