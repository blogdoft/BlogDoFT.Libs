using BlogDoFT.Libs.WarmUp.Constants;
using BlogDoFT.Libs.WarmUp.HealthChecks;
using BlogDoFT.Libs.WarmUp.HostedServices;
using BlogDoFT.Libs.WarmUp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;

namespace BlogDoFT.Libs.WarmUp.Extensions;

#pragma warning disable CA2254 // Template should be a static expression

/// <summary>
/// Provides extension methods to register and expose the application warm-up feature.
/// </summary>
[ExcludeFromCodeCoverage]
public static class WarmUpServiceExtension
{
    /// <summary>
    /// Registers the warm-up health check and hosted service, using an <see cref="ILoggerFactory"/> resolved
    /// from the application's service provider (at DI-resolve time, not registration time) to log warm-up progress.
    /// </summary>
    /// <param name="services">The service collection to add the warm-up registrations to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, so calls can be chained.</returns>
    public static IServiceCollection AddWarmUp(
        this IServiceCollection services) =>
        services
            .AddSingleton<WarmUpHealthCheck>()
            .AddHostedService<WarmUpHostedService>()
            .ConfigureHealthCheck()
            .AddWarmServices();

    /// <summary>
    /// Registers the warm-up health check and hosted service, using the supplied delegates to log warm-up progress.
    /// </summary>
    /// <param name="services">The service collection to add the warm-up registrations to.</param>
    /// <param name="logInfo">The delegate used to log informational messages.</param>
    /// <param name="logError">The delegate used to log error messages.</param>
    /// <param name="logTrace">The delegate used to log trace messages.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, so calls can be chained.</returns>
    public static IServiceCollection AddWarmUp(
        this IServiceCollection services,
        Action<string> logInfo,
        Action<string> logError,
        Action<string> logTrace) =>
        services
            .AddSingleton<WarmUpHealthCheck>()
            .AddHostedService<WarmUpHostedService>()
            .ConfigureHealthCheck()
            .AddWarmServices(logInfo, logError, logTrace);

    /// <summary>
    /// Registers the warm-up health check with the health checks system, tagged so it can be filtered by <see cref="UseWarmUp"/>.
    /// </summary>
    /// <param name="services">The service collection to add the health check registration to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, so calls can be chained.</returns>
    public static IServiceCollection ConfigureHealthCheck(
        this IServiceCollection services) =>
        services
            .AddHealthChecks()
                .AddCheck<WarmUpHealthCheck>(
                    WarmUpConstants.WarmUpName,
                    failureStatus: HealthStatus.Degraded,
                    tags: [WarmUpConstants.Tag])
                .Services;

    /// <summary>
    /// Maps a health check endpoint that reports the status of the warm-up health checks as JSON.
    /// </summary>
    /// <param name="app">The application builder to register the endpoint on.</param>
    /// <param name="route">The route pattern at which the warm-up health check endpoint is exposed.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance, so calls can be chained.</returns>
    public static IApplicationBuilder UseWarmUp(this IApplicationBuilder app, string route) =>
        app
            .UseHealthChecks(route, new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains(WarmUpConstants.Tag),
                ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                statusApplication = report.Status.ToString(),
                                healthChecks = report.Entries.Select(e => new
                                {
                                    check = e.Key,
                                    ErrorMessage = e.Value.Exception?.Message,
                                    status = Enum.GetName(e.Value.Status),
                                }),
                            });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    },
            });

    private static IServiceCollection AddWarmServices(
        this IServiceCollection services,
        Action<string> logInfo,
        Action<string> logError,
        Action<string> logTrace) =>
        services
            .AddTransient(provider => new PreloadingCommand(
                services,
                provider,
                logInfo,
                logError,
                logTrace))
            .AddTransient(provider => new WarmUpExecutor(
                services,
                provider,
                logInfo,
                logError,
                logTrace,
                provider.GetRequiredService<WarmUpHealthCheck>()));

    // Resolves the ILoggerFactory from the real provider each command is built with, at DI-resolve time
    // (host startup), instead of building a throwaway provider from the still-being-configured collection.
    private static IServiceCollection AddWarmServices(this IServiceCollection services) =>
        services
            .AddTransient(provider =>
            {
                var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("WarmUp");
                return new PreloadingCommand(
                    services,
                    provider,
                    message => logger.LogInformation(message),
                    message => logger.LogError(message),
                    message => logger.LogTrace(message));
            })
            .AddTransient(provider =>
            {
                var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("WarmUp");
                return new WarmUpExecutor(
                    services,
                    provider,
                    message => logger.LogInformation(message),
                    message => logger.LogError(message),
                    message => logger.LogTrace(message),
                    provider.GetRequiredService<WarmUpHealthCheck>());
            });
}
#pragma warning restore CA2254 // Template should be a static expression
