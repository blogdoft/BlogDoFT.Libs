using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.WarmUp.Services;

[ExcludeFromCodeCoverage]
internal class PreloadingCommand : BaseWarmCommand
{
    private const string LogMessage = "Pre-loading: {0}.";
    private const string LogErrorMessage = "Error when pre-loading {0}: {1}";
    private const string PreloadingStart = "Preloading services.";
    private const string PreloadingEnd = "Preloading services finished.";

    public PreloadingCommand(
        IServiceCollection services,
        IServiceProvider provider,
        Action<string> logInfo,
        Action<string> logError,
        Action<string> logTrace)
        : base(services, provider, logInfo, logError, logTrace)
    {
    }

    public override Task Execute()
    {
        LogInfo(PreloadingStart);
        using var scope = Provider.CreateScope();
        foreach (var type in GetServices())
        {
            try
            {
                scope.ServiceProvider.GetServices(type);

                var logMessage = string.Format(LogMessage, type.FullName);
                LogTrace(logMessage);
            }
            catch (Exception exception)
            {
                LogError(string.Format(LogErrorMessage, type.FullName, exception.Message));
            }
        }

        LogInfo(PreloadingEnd);

        return Task.CompletedTask;
    }

    private IEnumerable<Type> GetServices() =>
        Services
            .Where(
                descriptor => descriptor.ImplementationType != typeof(PreloadingCommand)
                && !descriptor.ServiceType.ContainsGenericParameters)
            .Select(descriptor => descriptor.ServiceType)
            .Distinct();
}
