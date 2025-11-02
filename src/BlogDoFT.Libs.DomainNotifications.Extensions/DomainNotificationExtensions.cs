using BlogDoFT.Libs.ResultPattern;

namespace BlogDoFT.Libs.DomainNotifications.Extensions;

public static class DomainNotificationExtensions
{
    public static void Add(this IDomainNotifications domainNotifications, Result result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        domainNotifications.Add(message: result.Failure.Message, code: result.Failure.Code);
    }

    public static void Add(this IDomainNotifications domainNotifications, Failure failure)
    {
        if (failure == Failure.None)
        {
            return;
        }

        domainNotifications.Add(message: failure.Message, code: failure.Code);
    }

    public static void Add<T>(this IDomainNotifications domainNotifications, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        domainNotifications.Add(result.Failure);
    }

    public static void Add(
        this IDomainNotifications domainNotifications,
        Exception exception,
        string? code = null)
    {
        domainNotifications.Add(message: exception.Message, code);
    }
}
