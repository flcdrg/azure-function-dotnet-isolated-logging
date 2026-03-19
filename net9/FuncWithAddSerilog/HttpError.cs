using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FuncWithAddSerilog;

public class HttpError(ILogger<HttpError> logger)
{
    [Function(nameof(HttpError))]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");
        throw new InvalidOperationException("Oh dear!");
    }
}