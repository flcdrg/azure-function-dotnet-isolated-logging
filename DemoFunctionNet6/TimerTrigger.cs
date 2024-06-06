using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DemoFunctionNet6
{
    public class TimerTrigger
    {
        private readonly ILogger _logger;

        public TimerTrigger(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TimerTrigger>();
        }

        [Function("TimerTrigger")]
        public void Run([TimerTrigger("0 */1 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogWarning($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
