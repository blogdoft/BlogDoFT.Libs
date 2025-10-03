using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlogDoFT.Libs.WarmUp.HealthChecks;

internal class WarmUpHealthCheck : IHealthCheck
{
    private volatile bool _warmUpCompleted = false;

    public bool WarmUpCompleted
    {
        get => _warmUpCompleted;
        set => _warmUpCompleted = value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (WarmUpCompleted)
        {
            return Task.FromResult(HealthCheckResult.Healthy("The warmup task is finished."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy("The warmup task is still running."));
    }
}
