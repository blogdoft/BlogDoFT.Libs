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
[ExcludeFromCodeCoverage]
public static class WarmUpServiceExtension
{
    public static IServiceCollection AddWarmUp(
        this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("WarmUp");

        void LogInfo(string logInfo) => logger.LogInformation(logInfo);
        void LogError(string logError) => logger.LogError(logError);
        void LogTrace(string logTrace) => logger.LogTrace(logTrace);

        return services.AddWarmUp(LogInfo, LogError, LogTrace);
    }

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

    public static IServiceCollection ConfigureHealthCheck(
        this IServiceCollection services) =>
        services
            .AddHealthChecks()
                .AddCheck<WarmUpHealthCheck>(
                    WarmUpConstants.WarmUpName,
                    failureStatus: HealthStatus.Degraded,
                    tags: [WarmUpConstants.Tag])
                .Services;

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
}
#pragma warning restore CA2254 // Template should be a static expression
