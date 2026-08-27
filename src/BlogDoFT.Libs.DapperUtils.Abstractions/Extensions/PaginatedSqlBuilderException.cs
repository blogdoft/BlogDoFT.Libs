using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.DapperUtils.Abstractions.Extensions;

/// <summary>
/// Represents errors raised while building a paginated SQL query.
/// </summary>
[ExcludeFromCodeCoverage]
[Serializable]
public class PaginatedSqlBuilderException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedSqlBuilderException"/> class.
    /// </summary>
    public PaginatedSqlBuilderException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedSqlBuilderException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PaginatedSqlBuilderException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedSqlBuilderException"/> class with a specified error message
    /// and a reference to the inner exception that caused this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public PaginatedSqlBuilderException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
