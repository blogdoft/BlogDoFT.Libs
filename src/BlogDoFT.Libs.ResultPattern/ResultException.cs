namespace BlogDoFT.Libs.ResultPattern;

public class ResultException : Exception
{
    protected ResultException(string message)
        : base(message)
    {
    }

    protected ResultException()
        : base()
    {
    }

    protected ResultException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal static ResultException CallValueOnFailure() =>
        new("Code calls \"Value\" property in a failure result.");

    internal static ResultException CallFailureOnSuccess() =>
        new("Code calls \"Value\" property in a successfull result.");
}
