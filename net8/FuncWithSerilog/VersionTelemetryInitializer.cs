using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace FuncWithSerilog;

public class VersionTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Component.Version = ThisAssembly.AssemblyFileVersion;
    }
}