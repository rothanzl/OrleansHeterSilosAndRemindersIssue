using System.Diagnostics;
using Abstractions;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Tester.Bomber.Scenarios;

public class SelfLoadingScenario : BaseScenarioMethod
{
    
    private readonly StartTestRequest _startTestRequest;
    private readonly Stopwatch _loggingSw = new();


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
        
        _loggingSw.Restart();
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
     
        int responseLimitMs, expectedSilos;
        bool printLog;
        lock (Mutex)
        {
            expectedSilos = _startTestRequest.ExpectedSilos;
            responseLimitMs = _startTestRequest.ResponseLimitMs;
            ActivatedGrains = counters!.ActivatedTestGrainCount;
            printLog = _loggingSw.ElapsedMilliseconds > 2000;
            if(printLog)
                _loggingSw.Restart();
        }
        
        if(printLog)
            _logger.LogInformation("Created grains {No}, last latency {Ms} ms", 
                counters.ActivatedTestGrainCount.ToString(), sw.ElapsedMilliseconds);
        

        if (counters.SystemHosts != expectedSilos)
            return Response.Fail(statusCode: "ErrorExpectedSilos" + counters.SystemHosts.ToString());
        
        if(responseLimitMs > 0 && sw.ElapsedMilliseconds > responseLimitMs)
            return Response.Fail(statusCode: "RestApiResponse" + sw.ElapsedMilliseconds.ToString());

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