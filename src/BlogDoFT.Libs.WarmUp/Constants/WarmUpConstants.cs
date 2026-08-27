namespace BlogDoFT.Libs.WarmUp.Constants;

/// <summary>
/// Provides constant values shared across the warm-up health check and its registration.
/// </summary>
public static class WarmUpConstants
{
    /// <summary>
    /// The health check tag used to filter warm-up related health checks.
    /// </summary>
    public const string Tag = "warmup";

    /// <summary>
    /// The name under which the warm-up health check is registered.
    /// </summary>
    public const string WarmUpName = "warmup_startup_healthcheck";
}
