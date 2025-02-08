using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddSerilog((serviceProvider, loggerConfiguration) =>
    {
        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ExtraInfo", "FuncWithSerilog")

            .WriteTo.ApplicationInsights(
                serviceProvider.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces);
    })
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
