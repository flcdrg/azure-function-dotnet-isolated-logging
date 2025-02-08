using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Func
{
    public class Timer
    {
#if DEBUG
        private const string Schedule = "0 */1 * * * *"; // every 1 minute
#else
        private const string Schedule = "0 */30 * * * *"; // every 30 minutes
#endif

        private readonly ILogger _logger;

        public Timer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Timer>();
        }

        [Function("Timer")]
        public void Run([TimerTrigger(Schedule, RunOnStartup = true)] TimerInfo myTimer, CancellationToken token)
        {
            _logger.LogInformation("C# Timer trigger function executed at: {Time}", DateTime.Now);

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("Next timer schedule at: {Time}", myTimer.ScheduleStatus.Next);
            }

            if (token.IsCancellationRequested)
            {
                _logger.LogInformation("Cancellation requested");
            }
        }
    }
}
