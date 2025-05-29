// using System.Threading.Tasks;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Azure.Functions.Worker.Http;
// using Microsoft.DurableTask;
// using Microsoft.DurableTask.Client;
// using Microsoft.Extensions.Logging;

// namespace bn.azurefuncs
// {
//     public static class Orchestrator1
//     {
//         [Function(nameof(Orchestrator1))]
//         public static async Task<List<string>> RunOrchestrator(
//             [OrchestrationTrigger] TaskOrchestrationContext context)
//         {
//             ILogger logger = context.CreateReplaySafeLogger(nameof(Orchestrator1));
//             logger.LogInformation("Saying hello from Orchestrator1.");
//             var outputs = new List<string>();

//             // Get the index passed when orchestration was scheduled
//             int index = context.GetInput<int>();

//             // Replace name and input with values relevant for your Durable Functions Activity
//             outputs.Add(await context.CallActivityAsync<string>("ActivityFunction", index));

//             return outputs;
//         }

//         [Function("Orchestrator1_HttpStart")]
//         public static async Task<HttpResponseData> HttpStart(
//             [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
//             [DurableClient] DurableTaskClient client,
//             FunctionContext executionContext)
//         {
//             ILogger logger = executionContext.GetLogger("Orchestrator1_HttpStart");

//             int totalRequests = 10; //10000

//             var tasks = new List<Task>();

//             for (int i = 0; i < totalRequests; i++)
//             {
//                 var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator1), i);
//                 logger.LogInformation($"Started orchestrator1 with ID = '{instanceId}'.");
//             }

//             var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
//             await response.WriteStringAsync($"{totalRequests} orchestrations triggered from Orchestrator1.");
//             return response;
//             // Returns an HTTP 202 response with an instance management payload.
//             // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
//             //return await client.CreateCheckStatusResponseAsync(req, instanceId);
//         }
//     }
// }
