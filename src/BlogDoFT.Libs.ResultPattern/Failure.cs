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

    public static Failure None => _none.Value;
    public static Failure DataNotFound => _dataNotFound.Value;
    public static Failure ValidationError => _validationError.Value;
}
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
