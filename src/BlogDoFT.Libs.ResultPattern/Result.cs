namespace BlogDoFT.Libs.ResultPattern;

/// <summary>
/// Represents the outcome of an operation that does not produce a value, indicating
/// either success or failure.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="failure">
    /// The failure associated with this result, or <see cref="Failure.None"/> to represent success.
    /// </param>
    protected Result(Failure failure)
    {
        Failure = failure;
    }

    /// <summary>
    /// Inform if operation has fail.
    /// </summary>
    public bool IsFailure => Failure != Failure.None;

    /// <summary>
    /// Inform if operation was executed successfully.
    /// </summary>
    public bool IsSuccess => !IsFailure;

    /// <summary>
    /// Return failure details.
    /// </summary>
    public Failure Failure { get; }

    /// <summary>
    /// Implicitly converts a <see cref="Failure"/> into a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="failure">The failure to wrap.</param>
    /// <returns>A <see cref="Result"/> representing the given failure.</returns>
    public static implicit operator Result(Failure failure) => new Result(failure);

    /// <summary>
    /// Creates a <see cref="Result"/> representing a successful operation.
    /// </summary>
    /// <returns>A <see cref="Result"/> whose <see cref="IsSuccess"/> is <see langword="true"/>.</returns>
    public static Result AsSuccess() => new(Failure.None);

    /// <summary>
    /// Creates a <see cref="Result"/> representing a failed operation.
    /// </summary>
    /// <param name="failure">The failure describing why the operation failed.</param>
    /// <returns>A <see cref="Result"/> whose <see cref="IsFailure"/> is <see langword="true"/>.</returns>
    public static Result AsFailure(Failure failure) => new(failure);
}

/// <summary>
/// Represents the outcome of an operation that produces a value of type
/// <typeparamref name="TValue"/> on success, or a <see cref="Failure"/> otherwise.
/// </summary>
/// <typeparam name="TValue">The type of the value produced when the operation succeeds.</typeparam>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(TValue value)
        : base(Failure.None)
    {
        _value = value;
    }

    private Result(Failure failure)
        : base(failure)
    {
        _value = default;
    }

    /// <summary>
    /// Return value.
    /// </summary>
    /// <remarks>
    /// In case of failure, a exception will be raised.
    /// </remarks>
    /// <exception cref="ResultException">Thrown when the result represents a failure.</exception>
    public TValue Value
    {
        get
        {
            if (!ReferenceEquals(Failure, Failure.None))
            {
#pragma warning disable S2372 // Exceptions should not be thrown from property getters
                throw ResultException.CallValueOnFailure();
#pragma warning restore S2372 // Exceptions should not be thrown from property getters
            }

            return _value!;
        }
    }

    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> into a successful
    /// <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The success value to wrap.</param>
    /// <returns>A <see cref="Result{TValue}"/> representing success with the given value.</returns>
    public static implicit operator Result<TValue>(TValue value) => FromSuccess(value);

    /// <summary>
    /// Implicitly converts a <see cref="Failure"/> into a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="failure">The failure to wrap.</param>
    /// <returns>A <see cref="Result{TValue}"/> representing the given failure.</returns>
    public static implicit operator Result<TValue>(Failure failure) => FromFailure(failure);

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> representing a successful operation.
    /// </summary>
    /// <param name="value">The value produced by the successful operation.</param>
    /// <returns>A <see cref="Result{TValue}"/> whose <see cref="Result.IsSuccess"/> is <see langword="true"/>.</returns>
    public static Result<TValue> FromSuccess(TValue value) => new(value);

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> representing a failed operation.
    /// </summary>
    /// <param name="failure">The failure describing why the operation failed.</param>
    /// <returns>A <see cref="Result{TValue}"/> whose <see cref="Result.IsFailure"/> is <see langword="true"/>.</returns>
    public static Result<TValue> FromFailure(Failure failure) => new(failure);
}
