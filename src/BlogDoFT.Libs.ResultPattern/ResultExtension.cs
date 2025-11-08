namespace BlogDoFT.Libs.ResultPattern;

public static class ResultExtension
{
    /// <summary>
    /// Executes the corresponding delegate depending on the success or failure state
    /// of the <paramref name="result"/>, returning a value of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type contained in the result.</typeparam>
    /// <param name="result">The result instance to inspect.</param>
    /// <param name="onSuccess">Executed when <paramref name="result"/> represents success.</param>
    /// <param name="onFailure">Executed when <paramref name="result"/> represents failure.</param>
    /// <returns>
    /// A value of type <typeparamref name="TValue"/> produced by the executed delegate.
    /// </returns>
    /// <remarks>
    /// Use this when you want to produce a final concrete value
    /// without continuing to compose additional results.
    /// </remarks>
    public static TValue On<TValue>(
        this Result<TValue> result,
        Func<TValue> onSuccess,
        Func<Failure, TValue> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Failure!);

    /// <summary>
    /// Executes the corresponding delegate depending on the success or failure state
    /// of the <paramref name="task"/>, returning a value of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type contained in the result.</typeparam>
    /// <param name="task">The result instance to inspect.</param>
    /// <param name="onSuccess">Executed when <paramref name="task"/> represents success.</param>
    /// <param name="onFailure">Executed when <paramref name="task"/> represents failure.</param>
    /// <returns>
    /// A value of type <typeparamref name="TValue"/> produced by the executed delegate.
    /// </returns>
    /// <remarks>
    /// Use this when you want to produce a final concrete value
    /// without continuing to compose additional results.
    /// </remarks>
    public static async Task<TValue> OnAsync<TValue>(
        this Task<Result<TValue>> task,
        Func<TValue> onSuccess,
        Func<Failure, TValue> onFailure)
    {
        var result = await task.ConfigureAwait(false);
        return result.IsSuccess ? onSuccess() : onFailure(result.Failure!);
    }
}
