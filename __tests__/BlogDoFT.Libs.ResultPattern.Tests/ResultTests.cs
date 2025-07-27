using BlogDoFT.Libs.ResultPattern.Tests.Fixtures;

namespace BlogDoFT.Libs.ResultPattern.Tests;

public class ResultTests
{
    private const int SuccessfullCall = 1;
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
        Result<Stub> result = new(new Stub());

        // When
        var isSuccess = result.IsSuccess;

        // Then
        isSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Shoule_IsSuccessBeFalse_When_ValueIsNull()
    {
        // Given
        Result<Stub> result = new(Failure.DataNotFound);

        // When
        var isSuccess = result.IsSuccess;

        // Then
        isSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Should_HasFailure_When_SuccessIsFalse()
    {
        // Given
        var failure = Failure.DataNotFound;
        var result = new Result<Stub>(failure);

        // When
        var stockFailure = result.Failure;

        // Then
        result.IsSuccess.ShouldBeFalse();
        stockFailure.ShouldBe(failure);
    }

    [Fact]
    public void Should_HasValue_When_SuccssIsTrue()
    {
        // Given
        var stub = new Stub();
        var result = new Result<Stub>(stub);

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
        var successfullResult = new Result<Stub>(new Stub());
        // When
        var _ = ReturnResult(successfullResult);

        // Then
        _functionCallPointer.ShouldBe(SuccessfullCall);
    }

    [Fact]
    public void Should_ChainFailCall_When_ResultIsFailure()
    {
        // Given
        var failureResult = new Result<Stub>(Failure.DataNotFound);

        // When
        var _ = ReturnResult(failureResult);

        // Then
        _functionCallPointer.ShouldBe(FailedCall);
    }

    [Fact]
    public void Should_ThrownException_When_TryGetValueOnFailure()
    {
        // Given
        Result<Stub> result = new(Failure.DataNotFound);

        // When
        Action act = () => _ = result.Value;

        // Then
        act.ShouldThrow<ResultException>();
    }

    [Fact]
    public void Should_MapResultToSuccessFullCall_When_IsSuccess()
    {
        // Given
        Result<Stub> result = new(new Stub());
        // When
        var callResult = result.Map(
            onSuccess: (_) => SuccessfullCall,
            onFailure: (_) => FailedCall);

        // Then
        callResult.ShouldBe(SuccessfullCall);
    }

    [Fact]
    public void Should_MapResultToFailedCall_When_IsAFailure()
    {
        // Given
        Result<Stub> result = new(Failure.ValidationError);

        // When
        var callResult = result.Map(
            onSuccess: (_) => SuccessfullCall,
            onFailure: (_) => FailedCall);

        // Then
        callResult.ShouldBe(FailedCall);
    }

    private Result<Stub> ReturnResult(Result<Stub> result)
    {
        result.On(
            onSuccess: () =>
            {
                _functionCallPointer = SuccessfullCall;
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
