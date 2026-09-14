using BlogDoFT.Libs.Flagr.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlogDoFT.Libs.Flagr;

/// <summary>
/// Extension methods for registering Flagr feature flag evaluation services into an
/// <see cref="IServiceCollection"/>.
/// </summary>
public static class ExtensionMethod
{
    private const string ConfigurationSectionName = "Flagr";

    /// <summary>
    /// Registers <see cref="IFlagEvaluator"/> and its <see cref="HttpClient"/>, binding
    /// <see cref="FlagrOptions"/> from the "Flagr" configuration section.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <returns>
    /// The <see cref="IHttpClientBuilder"/> for the registered <see cref="IFlagEvaluator"/>
    /// client, so callers can further customize it.
    /// </returns>
    public static IHttpClientBuilder AddFlagr(this IServiceCollection services)
    {
        services
            .AddOptions<FlagrOptions>()
            .Configure<IConfiguration>((options, configuration) =>
                configuration.GetSection(ConfigurationSectionName).Bind(options))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.BaseUrl),
                $"Missing or incomplete '{ConfigurationSectionName}' configuration section.");

        return ConfigureDeps(services);
    }

    /// <summary>
    /// Registers <see cref="IFlagEvaluator"/> and its <see cref="HttpClient"/>, configuring
    /// <see cref="FlagrOptions"/> programmatically instead of from configuration.
    /// </summary>
    /// <param name="services">The service collection to add the registrations to.</param>
    /// <param name="configureOptions">
    /// A delegate used to configure <see cref="FlagrOptions"/>, e.g.
    /// <c>options => options.BaseUrl = "https://flagr.example.com"</c>.
    /// </param>
    /// <returns>
    /// The <see cref="IHttpClientBuilder"/> for the registered <see cref="IFlagEvaluator"/>
    /// client, so callers can further customize it.
    /// </returns>
    public static IHttpClientBuilder AddFlagr(this IServiceCollection services, Action<FlagrOptions> configureOptions)
    {
        services.Configure(configureOptions);

        return ConfigureDeps(services);
    }

    private static IHttpClientBuilder ConfigureDeps(IServiceCollection services)
    {
        return services
            .AddHttpClient<IFlagEvaluator, FlagEvaluator>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<FlagrOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            });
    }
}
