namespace BlogDoFT.Libs.ResultPattern;

/// <summary>
/// Exception thrown when a <see cref="Result{TValue}"/> is used in a way that is not
/// consistent with its success or failure state, such as reading <see cref="Result{TValue}.Value"/>
/// on a failed result.
/// </summary>
public class ResultException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a
    /// specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ResultException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class.
    /// </summary>
    protected ResultException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultException"/> class with a
    /// specified error message and a reference to the inner exception that caused it.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ResultException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    internal static ResultException CallValueOnFailure() =>
        new("Code calls \"Value\" property in a failure result.");

    internal static ResultException CallFailureOnSuccess() =>
        new("Code calls \"Value\" property in a successfull result.");
}
