namespace BlogDoFT.Libs.ResultPattern;

public static class ThenExtension
{
    /// <summary>
    /// Chains another operation that also returns <see cref="Result{T}"/>,
    /// automatically propagating the failure without invoking the next step.
    /// </summary>
    /// <typeparam name="TIn">The input type of the current result.</typeparam>
    /// <typeparam name="TOut">The output type of the next result.</typeparam>
    /// <param name="result">The current result instance.</param>
    /// <param name="next">The function that takes the success value and produces the next result.</param>
    /// <returns>
    /// The <see cref="Result{TOut}"/> produced by <paramref name="next"/> if successful,
    /// or the current failure otherwise.
    /// </returns>
    /// <remarks>
    /// This is the core monadic operation: it flattens chained results and
    /// prevents nested <c>if</c> or <c>try/catch</c> blocks.
    /// </remarks>
    public static Result<TOut> Then<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> next)
    {
        if (result.IsFailure)
        {
            return result.Failure;
        }

        return next(result.Value!);
    }

    /// <summary>
    /// Asynchronous version of <see cref="Then{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/>
    /// for chaining tasks that produce <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="TIn">The input type of the current result.</typeparam>
    /// <typeparam name="TOut">The output type of the next result.</typeparam>
    /// <param name="task">The asynchronous operation producing a <see cref="Result{TIn}"/>.</param>
    /// <param name="next">
    /// An asynchronous function that takes the success value and produces a <see cref="Result{TOut}"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resolving to <see cref="Result{TOut}"/>,
    /// representing either the next result or the propagated failure.
    /// </returns>
    /// <remarks>
    /// This allows composing async methods without losing the success/failure semantics.
    /// </remarks>
    public static async Task<Result<TOut>> ThenAsync<TIn, TOut>(
        this Task<Result<TIn>> task,
        Func<TIn, Task<Result<TOut>>> next)
    {
        var result = await task.ConfigureAwait(false);
        if (result.IsFailure)
        {
            return result.Failure;
        }

        return await next(result.Value!).ConfigureAwait(false);
    }

    /// <summary>
    /// Asynchronous overload of <see cref="Then{TIn, TOut}(Result{TIn}, Func{TIn, Result{TOut}})"/>
    /// where the current result is synchronous but the next step is asynchronous.
    /// </summary>
    /// <typeparam name="TIn">The input type of the current result.</typeparam>
    /// <typeparam name="TOut">The output type of the next result.</typeparam>
    /// <param name="result">The current (synchronous) result.</param>
    /// <param name="next">
    /// An asynchronous function that takes the success value and produces a <see cref="Result{TOut}"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resolving to <see cref="Result{TOut}"/>,
    /// either the computed result or the propagated failure.
    /// </returns>
    public static async Task<Result<TOut>> ThenAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> next)
    {
        if (result.IsFailure)
        {
            return result.Failure;
        }

        return await next(result.Value!).ConfigureAwait(false);
    }

    /// <summary>
    /// Convenience asynchronous overload where the current operation is asynchronous
    /// but the next function is synchronous.
    /// </summary>
    /// <typeparam name="TIn">The input type of the current result.</typeparam>
    /// <typeparam name="TOut">The output type of the next result.</typeparam>
    /// <param name="task">The asynchronous operation producing <see cref="Result{TIn}"/>.</param>
    /// <param name="next">
    /// A synchronous function that takes the success value and produces <see cref="Result{TOut}"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resolving to <see cref="Result{TOut}"/>,
    /// either the computed result or the propagated failure.
    /// </returns>
    public static async Task<Result<TOut>> ThenAsync<TIn, TOut>(
        this Task<Result<TIn>> task,
        Func<TIn, Result<TOut>> next)
    {
        var result = await task.ConfigureAwait(false);
        if (result.IsFailure)
        {
            return result.Failure;
        }

        return next(result.Value!);
    }
}
