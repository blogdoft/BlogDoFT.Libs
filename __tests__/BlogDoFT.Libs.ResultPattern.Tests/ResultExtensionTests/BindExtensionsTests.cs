#pragma warning disable SA1313  // Parameter should begin with lower-case letter
namespace BlogDoFT.Libs.ResultPattern.Tests.ResultExtensionTests;

public class BindExtensionsTests
{
    [Fact]
    public void Should_CallNextAndReturnResult_When_BindSucceeds()
    {
        // Given
        Result<int> initial = 10;

        Result<string> Next(int x) => $"val:{x}";

        // When
        var result = initial.Bind<int, string>(Next);

        // Then
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("val:10");
    }

    [Fact]
    public void Should_PropagateFailureAndSkipNext_When_BindFails()
    {
        // Given
        var failure = new Failure(Code: "code", Message: "boom");
        Result<int> initial = failure;

        var called = 0;
        Result<string> Next(int _)
        {
            called++;
            return "should not be called";
        }

        // When
        var result = initial.Bind<int, string>(Next);

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.Failure.ShouldBeEquivalentTo(failure);
        called.ShouldBe(0);
    }

    [Fact]
    public async Task Should_CallNextAsync_When_TaskBindSucceedsAsync()
    {
        // Given
        Task<Result<int>> task = Task.FromResult<Result<int>>(7);

        Task<Result<string>> NextAsync(int x)
            => Task.FromResult<Result<string>>($"ok:{x}");

        // When
        var result = await task.BindAsync<int, string>(NextAsync);

        // Then
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("ok:7");
    }

    [Fact]
    public async Task Should_PropagateFailureAndSkipNext_When_TaskBindFailsAsync()
    {
        // Given
        var failure = new Failure(Code: "code", Message: "nope");
        Task<Result<int>> task = Task.FromResult<Result<int>>(failure);

        var called = 0;
        Task<Result<string>> NextAsync(int _)
        {
            called++;
            return Task.FromResult<Result<string>>("should not");
        }

        // When
        var result = await task.BindAsync<int, string>(NextAsync);

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.Failure.ShouldBeSameAs(failure);
        called.ShouldBe(0);
    }

    [Fact]
    public async Task Should_CallNextAsync_When_ResultBindSucceedsAsync()
    {
        // Given
        Result<int> result = 42;

        Task<Result<string>> NextAsync(int x)
            => Task.FromResult<Result<string>>((x * 2).ToString());

        // When
        var finalResult = await result.BindAsync<int, string>(NextAsync);

        // Then
        finalResult.IsSuccess.ShouldBeTrue();
        finalResult.Value.ShouldBe("84");
    }

    [Fact]
    public async Task Should_PropagateFailure_When_ResultBindFailsAsync()
    {
        // Given
        var failure = new Failure(Code: "code", Message: "fail");
        Result<int> result = failure;

        var called = 0;
        Task<Result<string>> NextAsync(int _)
        {
            called++;
            return Task.FromResult<Result<string>>("should not");
        }

        // When
        var finalResult = await result.BindAsync<int, string>(NextAsync);

        // Then
        finalResult.IsSuccess.ShouldBeFalse();
        finalResult.Failure.ShouldBeSameAs(failure);
        called.ShouldBe(0);
    }

    [Fact]
    public async Task Should_CallNext_When_TaskBindWithSyncNextSucceedsAsync()
    {
        // Given
        Task<Result<int>> task = Task.FromResult<Result<int>>(3);

        Result<string> Next(int x) => (Result<string>)$"n:{x}";

        // When
        var result = await task.BindAsync<int, string>(Next);

        // Then
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("n:3");
    }

    [Fact]
    public async Task Should_PropagateFailureAndSkipNext_When_TaskBindWithSyncNextFailsAsync()
    {
        // Given
        var failure = new Failure(Code: "code", Message: "bad");
        Task<Result<int>> task = Task.FromResult<Result<int>>(failure);

        var called = 0;
        Result<string> Next(int _)
        {
            called++;
            return (Result<string>)"should not";
        }

        // When
        var result = await task.BindAsync<int, string>(Next);

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.Failure.ShouldBeSameAs(failure);
        called.ShouldBe(0);
    }

    [Fact]
    public void Should_PassThroughFailure_When_NextReturnsFailure()
    {
        // Given
        var initial = (Result<int>)1;
        var nextFailure = new Failure(Code: "Code", Message: "downstream");

        Result<string> Next(int _) => (Result<string>)nextFailure;

        // When
        var result = initial.Bind<int, string>(Next);

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.Failure.ShouldBeSameAs(nextFailure);
    }

    [Fact]
    public async Task Should_PassThroughFailure_When_AsyncNextReturnsFailureAsync()
    {
        // Given
        Task<Result<int>> task = Task.FromResult<Result<int>>(5);
        var nextFailure = new Failure(Code: "code", Message: "downstream-async");

        Task<Result<string>> NextAsync(int _) => Task.FromResult<Result<string>>(nextFailure);

        // When
        var result = await task.BindAsync<int, string>(NextAsync);

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.Failure.ShouldBeSameAs(nextFailure);
    }
}
#pragma warning restore SA1313  // Parameter should begin with lower-case letter