using BlogDoFT.Libs.WarmUp.Services;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace BlogDoFT.Libs.WarmUp.HostedServices;

[ExcludeFromCodeCoverage]
internal class WarmUpHostedService : BackgroundService
{
    private readonly IImmutableList<BaseWarmCommand> _commands;

    public WarmUpHostedService(
        PreloadingCommand preloading,
        WarmUpExecutor warmUpExecutor) =>
        _commands = ImmutableList.Create<BaseWarmCommand>(
            preloading,
            warmUpExecutor);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        foreach (var command in _commands)
        {
            await command.Execute();
        }
    }
}
