using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace bn.azurefuncs;

public class RouterFunction
{
    private static int counter = 0;
    private static readonly HttpClient client = new HttpClient();

    [Function("RouterFunction")]
    public static async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", "get")] HttpRequestData req,
        FunctionContext context)
    {
        var logger = context.GetLogger("RouterFunction");

        // Round-robin logic
        var selectedApp = Interlocked.Increment(ref counter) % 2 == 0 ? "funcapp1" : "funcapp2";

        string targetUrl = selectedApp == "funcapp1"
            ? "https://orchestratorapp1.azurewebsites.net/api/Orchestrator1_HttpStart"
            : "https://orchestratorapp2.azurewebsites.net/api/Orchestrator2_HttpStart";

        logger.LogInformation($"Routing request to {selectedApp}: {targetUrl}");

        // Forward request and return the response
        var forwardedRequest = new HttpRequestMessage(HttpMethod.Post, targetUrl)
        {
            Content = new StreamContent(req.Body)
        };
        forwardedRequest.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var response = await client.SendAsync(forwardedRequest);
        var routerResponse = req.CreateResponse(response.StatusCode);
        await routerResponse.WriteStringAsync(await response.Content.ReadAsStringAsync());

        return routerResponse;
    }
}
