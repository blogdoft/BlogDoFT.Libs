namespace BlogDoFT.Libs.ResultPattern;

public static class TapFailureExtension
{
    /// <summary>
    /// Executes a side effect if the current <see cref="Result{T}"/> represents a failure,
    /// without modifying or replacing the result itself.
    /// </summary>
    /// <typeparam name="T">The type of the contained value when the result is successful.</typeparam>
    /// <param name="result">The result instance to inspect.</param>
    /// <param name="onFailure">
    /// The action to execute when the <paramref name="result"/> represents a failure.
    /// </param>
    /// <returns>
    /// The same <see cref="Result{T}"/> instance, allowing fluent chaining of operations.
    /// </returns>
    /// <remarks>
    /// Use this method to trigger notifications, logging, or other side effects
    /// when a failure occurs, while preserving the result pipeline intact.
    /// This method does not alter the original success or failure state.
    /// </remarks>
    public static Result<T> TapFailure<T>(this Result<T> result, Action<Failure> onFailure)
    {
        if (result.IsFailure)
        {
            onFailure(result.Failure);
        }

        return result;
    }

    /// <summary>
    /// Executes an asynchronous side effect if the current <see cref="Result{T}"/> represents a failure,
    /// without modifying or replacing the result itself.
    /// </summary>
    /// <typeparam name="T">The type of the contained value when the result is successful.</typeparam>
    /// <param name="task">The asynchronous operation that produces a <see cref="Result{T}"/>.</param>
    /// <param name="onFailureAsync">
    /// The asynchronous function to execute when the <see cref="Result{T}"/> represents a failure.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resolving to the same <see cref="Result{T}"/>,
    /// allowing fluent asynchronous chaining.
    /// </returns>
    /// <remarks>
    /// This method is useful for triggering asynchronous notifications, logging, or compensation actions
    /// when a failure occurs, without disrupting the original result pipeline.
    /// The returned <see cref="Result{T}"/> remains unchanged.
    /// </remarks>
    public static async Task<Result<T>> TapFailureAsync<T>(this Task<Result<T>> task, Func<Failure, Task> onFailureAsync)
    {
        var result = await task.ConfigureAwait(false);

        if (result.IsFailure)
        {
            await onFailureAsync(result.Failure).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Executes an asynchronous side effect if the current <see cref="Result{T}"/> represents a failure,
    /// without modifying or replacing the result itself.
    /// </summary>
    /// <typeparam name="T">The type of the contained value when the result is successful.</typeparam>
    /// <param name="result">The current <see cref="Result{T}"/> instance.</param>
    /// <param name="onFailureAsync">
    /// The asynchronous function to execute when the <see cref="Result{T}"/> represents a failure.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resolving to the same <see cref="Result{T}"/>,
    /// allowing fluent asynchronous chaining.
    /// </returns>
    /// <remarks>
    /// This overload is useful when you have a synchronous <see cref="Result{T}"/> in hand
    /// but need to perform an asynchronous action upon failure, such as logging or sending
    /// telemetry data to an external service.
    /// </remarks>
    public static async Task<Result<T>> TapFailureAsync<T>(this Result<T> result, Func<Failure, Task> onFailureAsync)
    {
        if (result.IsFailure)
        {
            await onFailureAsync(result.Failure).ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Executes a synchronous side effect if the awaited <see cref="Result{T}"/> represents a failure,
    /// without modifying or replacing the result itself.
    /// </summary>
    /// <typeparam name="T">The type of the contained value when the result is successful.</typeparam>
    /// <param name="task">The asynchronous operation producing a <see cref="Result{T}"/>.</param>
    /// <param name="onFailure">
    /// The synchronous action to execute when the <see cref="Result{T}"/> represents a failure.
    /// </param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> resolving to the same <see cref="Result{T}"/>,
    /// allowing fluent asynchronous chaining.
    /// </returns>
    /// <remarks>
    /// This overload avoids the need to return <see cref="Task.CompletedTask"/> when your failure handler is synchronous.
    /// </remarks>
    public static async Task<Result<T>> TapFailureAsync<T>(
        this Task<Result<T>> task,
        Action<Failure> onFailure)
    {
        var result = await task.ConfigureAwait(false);

        if (result.IsFailure)
        {
            onFailure(result.Failure);
        }

        return result;
    }
}
