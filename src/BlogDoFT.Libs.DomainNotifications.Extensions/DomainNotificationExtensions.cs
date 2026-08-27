using BlogDoFT.Libs.ResultPattern;

namespace BlogDoFT.Libs.DomainNotifications.Extensions;

/// <summary>
/// Provides extension methods to add domain notifications from <see cref="Result"/>, <see cref="Failure"/>,
/// and <see cref="Exception"/> instances.
/// </summary>
public static class DomainNotificationExtensions
{
    /// <summary>
    /// Adds a domain notification built from the failure of the specified <paramref name="result"/>.
    /// No notification is added when the result represents success.
    /// </summary>
    /// <param name="domainNotifications">The notification collection to add to.</param>
    /// <param name="result">The result to inspect.</param>
    public static void Add(this IDomainNotifications domainNotifications, Result result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        domainNotifications.Add(message: result.Failure.Message, code: result.Failure.Code);
    }

    /// <summary>
    /// Adds a domain notification built from the specified <paramref name="failure"/>.
    /// No notification is added when <paramref name="failure"/> is <see cref="Failure.None"/>.
    /// </summary>
    /// <param name="domainNotifications">The notification collection to add to.</param>
    /// <param name="failure">The failure to convert into a notification.</param>
    public static void Add(this IDomainNotifications domainNotifications, Failure failure)
    {
        if (failure == Failure.None)
        {
            return;
        }

        domainNotifications.Add(message: failure.Message, code: failure.Code);
    }

    /// <summary>
    /// Adds a domain notification built from the failure of the specified <paramref name="result"/>.
    /// No notification is added when the result represents success.
    /// </summary>
    /// <typeparam name="T">The type of the value produced by a successful result.</typeparam>
    /// <param name="domainNotifications">The notification collection to add to.</param>
    /// <param name="result">The result to inspect.</param>
    public static void Add<T>(this IDomainNotifications domainNotifications, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        domainNotifications.Add(result.Failure);
    }

    /// <summary>
    /// Adds a domain notification built from the message of the specified <paramref name="exception"/>.
    /// </summary>
    /// <param name="domainNotifications">The notification collection to add to.</param>
    /// <param name="exception">The exception whose message becomes the notification message.</param>
    /// <param name="code">An optional code to associate with the notification.</param>
    public static void Add(
        this IDomainNotifications domainNotifications,
        Exception exception,
        string? code = null)
    {
        domainNotifications.Add(message: exception.Message, code);
    }
}
