using BlogDoFT.Libs.Keycloak.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.Keycloak;

public static class ExtensionMethods
{
    private const string ConfigurationSectionName = "Keycloak";

    public static IServiceCollection AddKeycloakTokenProvider(this IServiceCollection services)
    {
        services
            .AddOptions<KeycloakOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(ConfigurationSectionName).Bind(options))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.TokenEndpoint)
                    && !string.IsNullOrWhiteSpace(options.ClientId)
                    && !string.IsNullOrWhiteSpace(options.ClientSecret),
                $"Missing or incomplete '{ConfigurationSectionName}' configuration section.");

        return ConfigureDeps(services);
    }

    public static IServiceCollection AddKeycloakTokenProvider(this IServiceCollection services, Action<KeycloakOptions> configureOptions)
    {
        services.Configure(configureOptions);

        return ConfigureDeps(services);
    }

    private static IServiceCollection ConfigureDeps(IServiceCollection services)
    {
        // The token endpoint authenticates via client_id/client_secret, not a Bearer
        // token, so KeycloakAuthenticationHandler must not wrap this client — doing so
        // would make fetching a token depend on already having one.
        services.AddHttpClient<IKeycloakTokenProvider, KeycloakTokenProvider>();
        services.AddTransient<KeycloakAuthenticationHandler>();

        return services;
    }
}
