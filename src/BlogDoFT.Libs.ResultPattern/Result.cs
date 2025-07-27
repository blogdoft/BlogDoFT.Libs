namespace BlogDoFT.Libs.ResultPattern;

public class Result<TValue>
{
    private readonly TValue? _value;

    public Result(TValue value)
    {
        _value = value;
        Failure = Failure.None;
    }

    public Result(Failure failure)
    {
        _value = default;
        Failure = failure;
    }

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

    public Failure Failure { get; }

    public bool IsSuccess => Failure == Failure.None;
}
