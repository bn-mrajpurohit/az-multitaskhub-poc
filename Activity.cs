using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace bn.azurefuncs
{
    public static class Activity
    {
        [Function("ActivityFunction")]
        public static async Task<string> RunAsync([ActivityTrigger] int index, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("ActivityFunction");
            logger.LogInformation($"[{index}] - Starting simulated workload.");
            await Task.Delay(TimeSpan.FromMinutes(2)); // Simulate 4 min processing
            logger.LogInformation($"[{index}] - Completed workload.");
            return $"Completed run index {index}!";
        }
    }
}
