using BlogDoFT.Libs.Flagr.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlogDoFT.Libs.Flagr;

public static class ExtensionMethod
{
    private const string ConfigurationSectionName = "Flagr";

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

    public static IHttpClientBuilder AddFlagr(this IServiceCollection services, Action<FlagrOptions> configureOptions)
    {
        services.Configure(configureOptions);

        return ConfigureDeps(services);
    }

    private static IHttpClientBuilder ConfigureDeps(IServiceCollection services)
    {
        return services
            .AddHttpClient<IFlagResolver, FlagResolver>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<FlagrOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            });

    }
}
