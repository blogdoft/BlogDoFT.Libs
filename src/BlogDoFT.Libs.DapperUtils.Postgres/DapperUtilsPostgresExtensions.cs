using BlogDoFT.Libs.DapperUtils.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.DapperUtils.Postgres;

[ExcludeFromCodeCoverage]
public static class DapperUtilsPostgresExtensions
{
    public static IServiceCollection AddDapperPostgres(this ServiceCollection services)
    {
        return services
            .AddSingleton<IConnectionFactory, NpgConnectionFactory>()
            .AddScoped<IDatabaseFacade, PostgresDatabaseFacade>();
    }

    public static IServiceCollection AddDapperPostgres(
        this ServiceCollection services,
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
