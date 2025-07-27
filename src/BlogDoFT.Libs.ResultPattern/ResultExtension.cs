namespace BlogDoFT.Libs.ResultPattern;

public static class ResultExtension
{
    public static TValue On<TValue>(
        this Result<TValue> result,
        Func<TValue> onSuccess,
        Func<Failure, TValue> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Failure!);

    public static TReturn Map<TValue, TReturn>(
        this Result<TValue> result,
        Func<TValue, TReturn> onSuccess,
        Func<Failure, TReturn> onFailure) =>
        result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.Failure!);
}
