using System.ComponentModel;
using System.Diagnostics;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Clients.MinimalApi.Bomber;


public record StartTestRequest(
    [property:DefaultValue(1)] int Rate, 
    [property:DefaultValue(0)] double RampMinutes, 
    [property:DefaultValue(60)] double DurationMinutes,
    [property:DefaultValue(0)] long CounterStartValue,
    [property:DefaultValue(100)] int MaxFailedCount,
    [property:DefaultValue(100)] int SubGrainsCount
    );

public class Tester
{
    private Task? _testTask;
    private Stopwatch _sw;
    private readonly string _testHostUrl;
    private readonly ILogger<Tester> _logger;

    public Tester(string testHostUrl, ILogger<Tester> logger)
    {
        _testHostUrl = testHostUrl;
        _logger = logger;
        _sw = new();
    }

    public StateResult Start(StartTestRequest req)
    {
        if(_testTask is {} && !_testTask.IsCompleted)
            return new StateResult(State: "Error: Test already running");
        
        _testTask = Task.Run(() => StartTests(req));
        return new StateResult(State: "Started");
    }


    private void StartTests(StartTestRequest req)
    {
        _sw.Restart();
        
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        using HttpClient httpClient = new(clientHandler);

        object mutex = new();
        long primGrainCounter = req.CounterStartValue;
        long totalActivatedGrains = 0;
        double subGrainCount = req.SubGrainsCount;
        TimeSpan lastDuration = TimeSpan.Zero;

        var cycleUpperThreshold = TimeSpan.FromMilliseconds(1000);
        var cycleLowerThreshold = TimeSpan.FromMilliseconds(950);
        
        
        async Task<IResponse> ExecutionMethod(IScenarioContext context)
        {
            long safePrimGrainCounter;
            int safeSubGrainCount;
            lock (mutex)
            {
                if (lastDuration > cycleUpperThreshold)
                {
                    subGrainCount *= 0.9;
                }else if (lastDuration < cycleLowerThreshold)
                {
                    subGrainCount *= 1.1;
                }

                if (subGrainCount < 1)
                    subGrainCount = 1;
                
                safePrimGrainCounter = primGrainCounter++;
                safeSubGrainCount = Convert.ToInt32(subGrainCount);
            }
            
            _logger.LogInformation("Prim grain {PrimGrain}, sub grains count {Count}", safePrimGrainCounter, safeSubGrainCount);
            
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var response = await httpClient.GetAsync($"https://{_testHostUrl}/hello/I{safePrimGrainCounter}/{safeSubGrainCount}");
                sw.Stop();

                lock (mutex)
                {
                    lastDuration = sw.Elapsed;

                    if (response.IsSuccessStatusCode)
                    {
                        totalActivatedGrains += safeSubGrainCount + 1;
                    }
                    
                }
                
                
                var responseContentBytes = await response.Content.ReadAsByteArrayAsync();

                return response.IsSuccessStatusCode
                    ? Response.Ok(
                        payload: safePrimGrainCounter, 
                        statusCode: response.StatusCode.ToString(), 
                        sizeBytes: safeSubGrainCount + 1)
                    : Response.Fail(
                        statusCode: response.StatusCode.ToString());
            }
            catch (Exception e)
            {
                return Response.Fail(message: e.ToString());
            }
        }

        var scenario = Scenario.Create("base_scenario", ExecutionMethod)
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.RampingInject(rate: req.Rate, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(req.RampMinutes)))
            .WithLoadSimulations(Simulation.Inject(rate: req.Rate, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(req.DurationMinutes)))
            .WithMaxFailCount(req.MaxFailedCount);


        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        _sw.Stop();
        
        _logger.LogWarning("Tester ended with {Count} total activated grains in {Time}", totalActivatedGrains, _sw.Elapsed);
    }

    public StateResult State()
    {
        if (_testTask is null)
            return new StateResult("No tests run");
        
        if(_testTask.IsCanceled)
            return new StateResult("Tests has been canceled");
        
        if(_testTask.IsFaulted)
            return new StateResult("Tests faulted: " + _testTask.Exception?.ToString());
        
        if(_testTask.IsCompleted)
            return new StateResult($"Tests completed in {_sw.Elapsed}");
        
        return new StateResult($"Tests running for {_sw.Elapsed}");
    }
}

public record StateResult(string State);