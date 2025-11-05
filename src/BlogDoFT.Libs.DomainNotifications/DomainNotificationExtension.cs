using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.DomainNotifications;

public static class DomainNotificationExtension
{
    public static IServiceCollection AddDomainNotification(this IServiceCollection services)
    {
        return services.AddScoped<IDomainNotifications, DomainNotificationBag>();
    }
}
