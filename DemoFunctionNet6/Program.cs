using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DemoFunctionNet6;
using Microsoft.EntityFrameworkCore;

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
    .Build();

host.Run();
