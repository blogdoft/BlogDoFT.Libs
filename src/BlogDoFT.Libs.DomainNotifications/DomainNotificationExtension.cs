using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.DomainNotifications;

/// <summary>
/// Provides extension methods to register domain notification services.
/// </summary>
public static class DomainNotificationExtension
{
    /// <summary>
    /// Registers <see cref="IDomainNotifications"/> as a scoped service backed by an internal notification bag.
    /// </summary>
    /// <param name="services">The service collection to add the registration to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, so calls can be chained.</returns>
    public static IServiceCollection AddDomainNotification(this IServiceCollection services)
    {
        return services.AddScoped<IDomainNotifications, DomainNotificationBag>();
    }
}
