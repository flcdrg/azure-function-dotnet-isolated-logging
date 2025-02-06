using Func;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.Extensibility;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<ITelemetryInitializer, VersionTelemetryInitializer>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
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

host.Run();