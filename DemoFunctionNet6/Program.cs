using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DemoFunctionNet6;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<BloggingContext>(builder =>
        {
            builder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test", optionsBuilder =>
            {
            });
        });
    })
    .ConfigureLogging(logging =>
    {
        //logging.ClearProviders();

        //logging.AddDebug();
        //logging.AddSystemdConsole();
        //logging.AddSimpleConsole(options => options.IncludeScopes = true);
        // Disable IHttpClientFactory Informational logs.
        // Note -- you can also remove the handler that does the logging: https://github.com/aspnet/HttpClientFactory/issues/196#issuecomment-432755765 
        logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

        // Remove the default Application Insights logger
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });

        // Exclude "Executed DbCommand" logs
        logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
    })
    .Build();

host.Run();
