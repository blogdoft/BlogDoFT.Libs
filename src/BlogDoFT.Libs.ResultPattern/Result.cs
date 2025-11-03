namespace BlogDoFT.Libs.ResultPattern;

public class Result
{
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

    public static implicit operator Result(Failure failure) => new Result(failure);

    public static Result AsSuccess() => new(Failure.None);

    public static Result AsFailure(Failure failure) => new(failure);
}

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

    public static implicit operator Result<TValue>(TValue value) => new(value);

    public static implicit operator Result<TValue>(Failure failure) => new(failure);
}
