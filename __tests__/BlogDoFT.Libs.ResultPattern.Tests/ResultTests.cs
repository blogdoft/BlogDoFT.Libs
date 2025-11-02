using BlogDoFT.Libs.ResultPattern.Tests.Fixtures;

namespace BlogDoFT.Libs.ResultPattern.Tests;

public class ResultTests
{
    private const int SuccessfulCall = 1;
    private const int FailedCall = -1;

    private int _functionCallPointer;

    public ResultTests()
    {
        _functionCallPointer = 0;
    }

    [Fact]
    public void Should_IsSuccessBeTrue_When_ValueIsNotNull()
    {
        // Given
        Result<Stub> result = new Stub();

        // When
        var isSuccess = result.IsSuccess;

        // Then
        isSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Should_IsSuccessBeFalse_When_ValueIsNull()
    {
        // Given
        Result<Stub> result = Failure.DataNotFound;

        // When
        var isSuccess = result.IsSuccess;

        // Then
        isSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Should_HasFailure_When_SuccessIsFalse()
    {
        // Given
        Result<Stub> result = Failure.DataNotFound;

        // When
        var stockFailure = result.Failure;

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        stockFailure.ShouldBe(Failure.DataNotFound);
    }

    [Fact]
    public void Should_HasValue_When_SuccessIsTrue()
    {
        // Given
        var stub = new Stub();
        Result<Stub> result = stub;

        // When
        var stockValue = result.Value;

        // Then
        result.IsSuccess.ShouldBeTrue();
        stockValue.ShouldBe(stub);
    }

    [Fact]
    public void Should_ChainSuccessCall_When_ResultIsSuccess()
    {
        // Given
        Result<Stub> successfulResult = new Stub();

        // When
        _ = ReturnResult(successfulResult);

        // Then
        _functionCallPointer.ShouldBe(SuccessfulCall);
    }

    [Fact]
    public void Should_ChainFailCall_When_ResultIsFailure()
    {
        // Given
        Result<Stub> failureResult = Failure.DataNotFound;

        // When
        _ = ReturnResult(failureResult);

        // Then
        _functionCallPointer.ShouldBe(FailedCall);
    }

    [Fact]
    public void Should_ThrownException_When_TryGetValueOnFailure()
    {
        // Given
        Result<Stub> result = Failure.DataNotFound;

        // When
        Action act = () => _ = result.Value;

        // Then
        act.ShouldThrow<ResultException>();
    }

    [Fact]
    public void Should_MapResultToSuccessFullCall_When_IsSuccess()
    {
        // Given
        Result<Stub> result = new Stub();

        // When
        var callResult = result.Map(
            onSuccess: (_) => SuccessfulCall,
            onFailure: (_) => FailedCall);

        // Then
        callResult.ShouldBe(SuccessfulCall);
    }

    [Fact]
    public void Should_MapResultToFailedCall_When_IsAFailure()
    {
        // Given
        Result<Stub> result = Failure.ValidationError;

        // When
        var callResult = result.Map(
            onSuccess: (_) => SuccessfulCall,
            onFailure: (_) => FailedCall);

        // Then
        callResult.ShouldBe(FailedCall);
    }

    [Fact]
    public void Should_ReturnSuccess_When_ResultIsVoid()
    {
        // Given
        // When
        var result = Result.AsSuccess();

        // Then
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Should_ReturnFailure_When_ResultIsVoid()
    {
        // Given
        // When
        var result = Result.AsFailure(Failure.DataNotFound);

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Should_ImplicitReturnFailure_When_ResultIsVoid()
    {
        // Given
        // When
        Result result = Failure.ValidationError;

        // Then
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.Failure.ShouldBe(Failure.ValidationError);
    }

    [Fact]
    public void Should_BeSuccessFul_When_FailureIsNone()
    {
        // Given
        // When
        Result implicitResult = Failure.None;

        // Then
        implicitResult.IsSuccess.ShouldBeTrue();
        implicitResult.IsFailure.ShouldBeFalse();
        implicitResult.Failure.ShouldBe(Failure.None);
    }

    private Result<Stub> ReturnResult(Result<Stub> result)
    {
        result.On(
            onSuccess: () =>
            {
                _functionCallPointer = SuccessfulCall;
                return result.Value;
            },
            onFailure: (_) =>
            {
                _functionCallPointer = FailedCall;
                return new Stub();
            });
        return result;
    }
}
