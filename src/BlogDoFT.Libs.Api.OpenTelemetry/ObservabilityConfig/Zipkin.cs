namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

/// <summary>
/// Configuration for a Zipkin exporter endpoint.
/// </summary>
public class Zipkin
{
    /// <summary>
    /// Gets or sets the Zipkin collector endpoint.
    /// </summary>
    public Uri Endpoint { get; set; } = null!;
}
