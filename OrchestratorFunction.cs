using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace bn.azurefuncs
{
    public static class OrchestratorFunction
    {
        [Function(nameof(OrchestratorFunction))]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(OrchestratorFunction));
            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(RunAsync), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(RunAsync), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(RunAsync), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [Function("ActivityFunction")]
        public static async Task<string> RunAsync([ActivityTrigger] int index, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("ActivityFunction");
            logger.LogInformation($"[{index}] - Starting simulated workload.");
            await Task.Delay(TimeSpan.FromMinutes(5)); // Simulate 5 min processing
            logger.LogInformation($"[{index}] - Completed workload.");
            return $"Completed run index {index}!";
        }

        [Function("OrchestratorFunction_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("OrchestratorFunction_HttpStart");

            int totalRequests = 200;

            var tasks = new List<Task>();

            for (int i = 0; i < totalRequests; i++)
            {
                var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrchestratorFunction), i);
                logger.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            }

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync($"{totalRequests} orchestrations triggered.");
            return response;
            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            //return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}
