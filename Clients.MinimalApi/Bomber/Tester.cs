using System.ComponentModel;
using System.Diagnostics;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Clients.MinimalApi.Bomber;


public record StartTestRequest(
    [property:DefaultValue(500)] int Rate, 
    [property:DefaultValue(0.5)] double RampMinutes, 
    [property:DefaultValue(30)] double DurationMinutes,
    [property:DefaultValue(0)] long CounterStartValue,
    [property:DefaultValue(100)] int MaxFailedCount);

public class Tester
{
    private Task? _testTask;
    private Stopwatch _sw;
    private readonly string _testHostUrl;

    public Tester(string testHostUrl)
    {
        _testHostUrl = testHostUrl;
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
        long counter = req.CounterStartValue;
        
        async Task<IResponse> ExecutionMethod(IScenarioContext context)
        {
            long safeCounter;
            lock (mutex)
            {
                safeCounter = counter++;
            }

            try
            {
                var response = await httpClient.GetAsync($"https://{_testHostUrl}/hello/I{safeCounter}");
                var responseContentBytes = await response.Content.ReadAsByteArrayAsync();

                return response.IsSuccessStatusCode
                    ? Response.Ok(payload: safeCounter, statusCode: response.StatusCode.ToString(), sizeBytes: responseContentBytes.Length)
                    : Response.Fail(statusCode: response.StatusCode.ToString());
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