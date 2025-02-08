using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Microsoft.ApplicationInsights;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

Log.Logger = new LoggerConfiguration()
    //.WriteTo.Console()
    //.WriteTo.Debug()
    .WriteTo.ApplicationInsights(new TelemetryClient(new TelemetryConfiguration()), new TraceTelemetryConverter())
    .CreateBootstrapLogger();

TelemetryClient? telemetryClient = null;
try
{
    Log.Warning("Starting up.."); // Only logged to console

    var build = Host.CreateDefaultBuilder(args)
        .UseSerilog((_, services, loggerConfiguration) => loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ExtraInfo", "FuncWithSerilog")

            .WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces))

        .ConfigureFunctionsWebApplication()

        .ConfigureServices(services => {
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();

            // Uncomment the next line to simulate a configuration error which will be logged by the bootstrap logger
            // throw new InvalidOperationException("This is a test exception");

        })
        .ConfigureLogging(logging =>
        {
            // Remove the default Application Insights logger provider so that Information logs are sent
            // https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Clinux&WT.mc_id=DOP-MVP-5001655#managing-log-levels
            logging.Services.Configure<LoggerFilterOptions>(options =>
            {
                LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                    == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                if (defaultRule is not null)
                {
                    options.Rules.Remove(defaultRule);
                }
            });
        })

        .Build();

    telemetryClient = build.Services.GetRequiredService<TelemetryClient>();

    build.Run();
    Log.Warning("After run");
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}
finally
{
    telemetryClient?.TrackTrace("Down we go");
    Log.Warning("Exiting application");
    Log.CloseAndFlush();
    telemetryClient?.Flush();
    Task.Delay(5000).Wait();
}

