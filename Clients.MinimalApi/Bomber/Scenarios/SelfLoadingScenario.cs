using System.Diagnostics;
using Abstractions;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Clients.MinimalApi.Bomber.Scenarios;

public class SelfLoadingScenario : BaseScenarioMethod
{
    
    private readonly StartTestRequest _startTestRequest;


    public SelfLoadingScenario(TesterConfig config, ILogger logger, StartTestRequest startTestRequest) : base(config, logger)
    {
        _startTestRequest = startTestRequest;
    }

    public override async Task Init()
    {
        using var httpClient = HttpClientFactory();
        var response = await httpClient.PutAsync($"{_config.TestAppUrl}/populate/start", null);
        _logger.LogInformation("Start populate with response {Code}", response.StatusCode.ToString());
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Start populate with response {response.StatusCode.ToString()}");
        }
    }

    public override async Task<IResponse> Method(IScenarioContext context)
    {
        using HttpClient httpClient = HttpClientFactory();
        Stopwatch sw = Stopwatch.StartNew();
        var response = await httpClient.GetAsync($"{_config.TestAppUrl}/counters");
        sw.Stop();
        
        if(!response.IsSuccessStatusCode)
            return Response.Fail(statusCode: response.StatusCode.ToString());


        var counters = await response.Content.ReadFromJsonAsync<CountersResponse>();
     
        int responseLimitMs;
        lock (Mutex)
        {
            responseLimitMs = _startTestRequest.ResponseLimitMs;
            ActivatedGrains = counters!.ActivatedTestGrainCount;
        }
        
        if(sw.ElapsedMilliseconds > responseLimitMs)
            return Response.Fail(statusCode: "RestApiResponse" + sw.ElapsedMilliseconds);

        return Response.Ok(statusCode: response.StatusCode.ToString());
    }

    

    public override async ValueTask DisposeAsync()
    {
        using var httpClient = HttpClientFactory();
        var response = await httpClient.PutAsync($"{_config.TestAppUrl}/populate/stop", null);
        _logger.LogInformation("Stop populate with response {Code}", response.StatusCode.ToString());

        await base.DisposeAsync();
    }
}