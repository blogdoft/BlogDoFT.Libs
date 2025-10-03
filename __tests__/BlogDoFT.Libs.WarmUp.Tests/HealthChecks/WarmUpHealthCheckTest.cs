using BlogDoFT.Libs.WarmUp.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlogDoFT.Libs.WarmUp.Tests.HealthChecks;

public class WarmUpHealthCheckTest
{
    [Fact]
    public async Task Should_ReturnUnhealthy_When_WarmUpIsNotCompletedAsync()
    {
        // Given
        var warmUpHealthCheck = new WarmUpHealthCheck
        {
            WarmUpCompleted = false,
        };

        // When
        var healthCheckResult = await warmUpHealthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Then
        healthCheckResult.Description.ShouldBe("The warmup task is still running.");
        healthCheckResult.Status.ShouldBe(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task Should_ReturnHealthy_When_WarmUpIsCompletedAsync()
    {
        // Given
        var warmUpHealthCheck = new WarmUpHealthCheck
        {
            WarmUpCompleted = true,
        };

        // When
        var healthCheckResult = await warmUpHealthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        // Then
        healthCheckResult.Description.ShouldBe("The warmup task is finished.");
        healthCheckResult.Status.ShouldBe(HealthStatus.Healthy);
    }
}
