using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Runtime.InteropServices;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var build = Host.CreateDefaultBuilder(args)
        .UseSerilog((_, services, loggerConfiguration) => loggerConfiguration
            .WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces))
        .ConfigureFunctionsWebApplication()
        .ConfigureServices(services => {
            services.AddApplicationInsightsTelemetryWorkerService();
        })

        .Build();

    //PosixSignalRegistration.Create(PosixSignal.SIGTERM, (ctx) =>
    //{
    //    Console.WriteLine("Caught SIGTERM, default host is shutting down");
    //});

    build.Run();
    Log.Information("After run");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}
finally
{
    Log.Information("Exiting application");
    Log.CloseAndFlush();
}
