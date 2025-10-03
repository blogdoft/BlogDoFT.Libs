using BlogDoFT.Libs.WarmUp.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.WarmUp.Services;

[ExcludeFromCodeCoverage]
internal class WarmUpExecutor : BaseWarmCommand
{
    private const string LogMessage = "Warming-up {0}.";
    private const string LogErrorMessage = "Error when warming-up {0}: {1}";
    private readonly WarmUpHealthCheck _warmUpHealthCheck;

    public WarmUpExecutor(
        IServiceCollection services,
        IServiceProvider provider,
        Action<string> logInfo,
        Action<string> logError,
        Action<string> logTrace,
        WarmUpHealthCheck warmUpHealthCheck)
        : base(services, provider, logInfo, logError, logTrace)
    {
        _warmUpHealthCheck = warmUpHealthCheck;
    }

    public override Task Execute()
    {
        LogInfo("Executing warmup.");
        Task.Run(async () =>
        {
            using var scope = Provider.CreateScope();
            foreach (var type in GetCommands())
            {
                if (scope.ServiceProvider.GetRequiredService(type) is not IWarmUpCommand warmUpCommand)
                {
                    continue;
                }

                await ExecuteWarmingUpAsync(warmUpCommand);
                var logMessage = string.Format(LogMessage, warmUpCommand.GetType().FullName);
                LogTrace(logMessage);
            }

            _warmUpHealthCheck.WarmUpCompleted = true;
            LogInfo("Warmup finished.");
        });

        return Task.CompletedTask;
    }

    private IEnumerable<Type> GetCommands() =>
        Services
            .Where(
                descriptor => descriptor.ImplementationType?
                    .GetInterfaces()
                    .Contains(typeof(IWarmUpCommand)) == true)
            .Select(descriptor => descriptor.ServiceType)
            .Distinct();

    private async Task ExecuteWarmingUpAsync(IWarmUpCommand command)
    {
        try
        {
            if (command is null)
            {
                return;
            }

            await command.Execute();
        }
        catch (Exception exception)
        {
            LogError(string.Format(LogErrorMessage, command.GetType().FullName, exception.Message));
        }
    }
}
