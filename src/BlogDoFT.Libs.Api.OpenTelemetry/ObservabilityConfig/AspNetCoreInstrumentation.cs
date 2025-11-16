using Microsoft.Extensions.Configuration;

namespace BlogDoFT.Libs.Api.OpenTelemetry.ObservabilityConfig;

public class AspNetCoreInstrumentation
{
    public AspNetCoreInstrumentation(IConfigurationSection section)
    {
        RecordException = section.GetValue(nameof(RecordException), true);
    }

    public bool RecordException { get; }
}
