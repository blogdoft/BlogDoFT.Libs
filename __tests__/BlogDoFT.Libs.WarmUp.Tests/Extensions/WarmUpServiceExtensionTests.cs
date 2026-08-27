using BlogDoFT.Libs.WarmUp.Extensions;
using BlogDoFT.Libs.WarmUp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlogDoFT.Libs.WarmUp.Tests.Extensions;

public class WarmUpServiceExtensionTests
{
    [Fact]
    public void Should_NotThrow_When_RegisteringWithoutAnILoggerFactoryAlreadyResolvable()
    {
        // Given
        var services = new ServiceCollection();

        // When
        var act = () => services.AddWarmUp();

        // Then
        Should.NotThrow(act);
    }

    [Fact]
    public void Should_ResolveWarmUpCommands_When_ProviderIsBuiltAfterRegistration()
    {
        // Given
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWarmUp();

        // When
        using var provider = services.BuildServiceProvider();
        var preloading = provider.GetRequiredService<PreloadingCommand>();
        var warmUpExecutor = provider.GetRequiredService<WarmUpExecutor>();

        // Then
        preloading.ShouldNotBeNull();
        warmUpExecutor.ShouldNotBeNull();
    }
}
