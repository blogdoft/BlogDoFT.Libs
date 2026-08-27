namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

/// <summary>
/// Configuration for an OTLP (OpenTelemetry Protocol) endpoint.
/// </summary>
public class Otlp
{
    /// <summary>
    /// Gets or sets the OTLP collector endpoint.
    /// </summary>
    public Uri Endpoint { get; set; } = null!;
}
