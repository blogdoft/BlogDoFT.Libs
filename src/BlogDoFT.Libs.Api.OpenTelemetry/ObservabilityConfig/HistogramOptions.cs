namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

public enum HistogramOptions
{
    /// <summary>
    /// Uses explicit bucket boundaries defined by the OpenTelemetry defaults.
    /// </summary>
    Explicit,

    /// <summary>
    /// Uses Base2 exponential bucket boundaries.
    /// </summary>
    Exponential,
}
