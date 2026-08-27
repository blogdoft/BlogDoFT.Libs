using BlogDoFT.Libs.DapperUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

[ExcludeFromCodeCoverage]
public static class DapperUtilsPostgresExtensions
{
    /// <summary>
    /// Registers the default Postgres-backed <see cref="IConnectionFactory"/> and <see cref="IDatabaseFacade"/> implementations.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies into.</param>
    /// <returns>The same service collection, to allow chaining.</returns>
    public static IServiceCollection AddDapperPostgres(this IServiceCollection services)
    {
        return services
            .AddSingleton<IConnectionFactory, NpgConnectionFactory>()
            .AddScoped<IDatabaseFacade, PostgresDatabaseFacade>();
    }

    /// <summary>
    /// Registers the default Postgres-backed <see cref="IDatabaseFacade"/> implementation and replaces the
    /// registered <see cref="IConnectionFactory"/> with the supplied instance.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies into.</param>
    /// <param name="connectionFactory">The connection factory instance to use instead of the default one.</param>
    /// <returns>The same service collection, to allow chaining.</returns>
    public static IServiceCollection AddDapperPostgres(
        this IServiceCollection services,
        IConnectionFactory connectionFactory)
    {
        services.AddDapperPostgres();

        var serviceLocator = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IConnectionFactory));

        if (serviceLocator is not null)
        {
            services.Remove(serviceLocator);
        }

        services.AddSingleton<IConnectionFactory>(_ => connectionFactory);

        return services;
    }
}
