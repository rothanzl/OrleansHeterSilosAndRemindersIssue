using System.Diagnostics;
using System.Globalization;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Clients.MinimalApi.Bomber.Scenarios;

public class HelloLoadScenario : BaseScenarioMethod
{
    public HelloLoadScenario(TesterConfig config, ILogger logger) : base(config, logger)
    {
        PrimGrainCounter = 0;
    }

    private int PrimGrainCounter { get; set; }

    public override async Task<IResponse> Method(IScenarioContext context)
    {
        HttpClient httpClient = GetHttpClient(context);
        
        long safePrimGrainCounter;
        lock (Mutex)
        {
            safePrimGrainCounter = PrimGrainCounter++;
        }

        try
        {
            Stopwatch sw = Stopwatch.StartNew();
            var response = await httpClient.GetAsync($"https://{_config.TestAppUrl}/hello/I{safePrimGrainCounter}");
            sw.Stop();

            int activatedGrains = 0;
            if (response.IsSuccessStatusCode)
            {
                var responseBodyString = await response.Content.ReadAsStringAsync();
                Int32.TryParse(responseBodyString, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out activatedGrains);

                lock (Mutex)
                {
                    ActivatedGrains += activatedGrains;
                }
            }

            _logger.LogInformation("Prim grain {PrimGrain}, activated grains {Count}", safePrimGrainCounter, activatedGrains);

            return activatedGrains > 2
                ? Response.Ok(
                    payload: safePrimGrainCounter, 
                    statusCode: response.StatusCode.ToString(),
                    message: $"ActivatedGrains-{activatedGrains}")
                : Response.Fail(
                    statusCode: response.StatusCode.ToString(),
                    message: $"ActivatedGrains-{activatedGrains}");
        }
        catch (Exception e)
        {
            return Response.Fail(message: e.ToString());
        }
    }
}