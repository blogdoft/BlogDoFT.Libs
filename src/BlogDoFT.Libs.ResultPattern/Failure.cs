#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
namespace BlogDoFT.Libs.ResultPattern;

/// <summary>
/// Base class to specify failures during processing data.
/// Use this class to extend more detailed errors. You may segment it by
/// system, micro service, or any other way that you want.
/// </summary>
/// <param name="Code">A code that specify you error. You may use this as a common identifier at logs,
/// or for translation on frontend.</param>
/// <param name="Message">A detailed failure description</param>

public record class Failure(string Code, string Message)
{
    private static readonly Lazy<Failure> _none = new(() => new(string.Empty, string.Empty));
    private static readonly Lazy<Failure> _dataNotFound = new(() => new("common-404", "Resource not found."));
    private static readonly Lazy<Failure> _validationError = new(() => new("common-400", "Validation error occurred."));

    /// <summary>
    /// Gets a <see cref="Failure"/> instance that represents the absence of a failure,
    /// used internally to signal a successful <see cref="Result"/>.
    /// </summary>
    public static Failure None => _none.Value;

    /// <summary>
    /// Gets a reusable <see cref="Failure"/> with code <c>common-404</c>, indicating that
    /// a requested resource could not be found.
    /// </summary>
    public static Failure DataNotFound => _dataNotFound.Value;

    /// <summary>
    /// Gets a reusable <see cref="Failure"/> with code <c>common-400</c>, indicating that
    /// input validation failed.
    /// </summary>
    public static Failure ValidationError => _validationError.Value;
}
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
