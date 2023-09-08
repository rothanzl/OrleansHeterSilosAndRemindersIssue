using System.Diagnostics;
using Abstractions;
using Abstractions.BroadcastChannel;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Tester.Bomber.Scenarios;

public class SelfLoadingScenario : BaseScenarioMethod
{
    
    private readonly StartTestRequest _startTestRequest;
    private readonly Stopwatch _loggingSw = new();

    private StatsResponse ChannelsStats { get; set; } = new(new(), new(), new());


    public SelfLoadingScenario(TesterConfig config, ILogger logger, StartTestRequest startTestRequest) : base(config, logger)
    {
        _startTestRequest = startTestRequest;
    }

    public override async Task Init()
    {
        using var httpClient = HttpClientFactory();
        var response = await httpClient.PostAsync($"{_config.TestAppUrl}/test/start", null);
        _logger.LogInformation("Start populate with response {@Code}", response.StatusCode.ToString());
        
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
        var countersResponse = await httpClient.GetAsync($"{_config.TestAppUrl}/counters");
        var channelsStatsResponse = await httpClient.GetAsync($"{_config.TestAppUrl}/broadcast");
        sw.Stop();
        
        if(!countersResponse.IsSuccessStatusCode)
            return Response.Fail(statusCode: countersResponse.StatusCode.ToString());
        
        if(!channelsStatsResponse.IsSuccessStatusCode)
            return Response.Fail(statusCode: channelsStatsResponse.StatusCode.ToString());


        var counters = await countersResponse.Content.ReadFromJsonAsync<CountersResponse>();
        var channelsStats = await channelsStatsResponse.Content.ReadFromJsonAsync<StatsResponse>();
        
        int responseLimitMs, expectedSilos;
        bool printLog;
        lock (Mutex)
        {
            expectedSilos = _startTestRequest.ExpectedSilos;
            responseLimitMs = _startTestRequest.ResponseLimitMs;
            printLog = _loggingSw.ElapsedMilliseconds > 2000;
            ChannelsStats = channelsStats!;
            if(printLog)
                _loggingSw.Restart();
        }
        
        if(printLog)
            _logger.LogInformation("Last latency {@Ms} ms, Channels stats: {@Ch}",
                sw.ElapsedMilliseconds, channelsStats);
        

        if (counters.SystemHosts != expectedSilos)
            return Response.Fail(statusCode: "ErrorExpectedSilos" + counters.SystemHosts.ToString());
        
        if(responseLimitMs > 0 && sw.ElapsedMilliseconds > responseLimitMs)
            return Response.Fail(statusCode: "RestApiResponse" + sw.ElapsedMilliseconds.ToString());

        return Response.Ok(statusCode: countersResponse.StatusCode.ToString());
    }

    public override void TestEndHook(TimeSpan testDuration)
    {
        _logger.LogWarning("Test durations {Sw}, channel stats: {Ch}", testDuration, ChannelsStats);
    }


    public override async ValueTask DisposeAsync()
    {
        using var httpClient = HttpClientFactory();
        var response = await httpClient.PostAsync($"{_config.TestAppUrl}/test/stop", null);
        _logger.LogInformation("Stop populate with response {Code}", response.StatusCode.ToString());

        await base.DisposeAsync();
    }
}