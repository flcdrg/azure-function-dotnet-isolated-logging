using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DemoFunctionNet6
{
    public class TimerTrigger
    {
        private readonly BloggingContext _context;
        private readonly ILogger _logger;

        public TimerTrigger(ILoggerFactory loggerFactory, BloggingContext context)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
        }

        [Function("TimerTrigger")]
        public void Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogWarning($"C# Timer trigger function executed at: {DateTime.Now}");

            // Note: This sample requires the database to be created before running.
            _logger.LogWarning($"Database path: {_context.DbPath}.");

            // Create
            _logger.LogInformation("Inserting data");
            _context.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
            _context.SaveChanges();

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
