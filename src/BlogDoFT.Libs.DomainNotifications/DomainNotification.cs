namespace BlogDoFT.Libs.DomainNotifications;

/// <summary>
/// Represents a single domain notification, carrying a message and an optional identifying code.
/// </summary>
public record DomainNotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainNotification"/> record.
    /// </summary>
    /// <param name="message">The notification message.</param>
    /// <param name="code">An optional code identifying the notification.</param>
    public DomainNotification(string message, string? code = null)
    {
        Message = message;
        Code = code;
    }

    /// <summary>
    /// Gets the code identifying the notification, if any.
    /// </summary>
    public string? Code { get; init; }

    /// <summary>
    /// Gets the notification message.
    /// </summary>
    public string Message { get; }
}
