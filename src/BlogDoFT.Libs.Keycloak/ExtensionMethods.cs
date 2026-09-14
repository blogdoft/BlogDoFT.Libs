using BlogDoFT.Libs.Keycloak.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.Keycloak;

/// <summary>
/// Extension methods for registering Keycloak token provider services into an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class ExtensionMethods
{
    private const string ConfigurationSectionName = "Keycloak";

    /// <summary>
    /// Registers <see cref="IKeycloakTokenProvider"/> and <see cref="KeycloakAuthenticationHandler"/>,
    /// binding <see cref="KeycloakOptions"/> from the "Keycloak" configuration section.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>The same <see cref="IServiceCollection"/>, for chaining.</returns>
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

    /// <summary>
    /// Registers <see cref="IKeycloakTokenProvider"/> and <see cref="KeycloakAuthenticationHandler"/>,
    /// configuring <see cref="KeycloakOptions"/> programmatically instead of from configuration.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configureOptions">
    /// A delegate used to configure <see cref="KeycloakOptions"/>, e.g.
    /// <c>options => options.ClientId = "my-service"</c>.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/>, for chaining.</returns>
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
