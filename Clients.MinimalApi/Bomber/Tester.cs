using System.Diagnostics;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Clients.MinimalApi.Bomber;

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

    public StateResult Start()
    {
        if(_testTask is {} && !_testTask.IsCompleted)
            return new StateResult(State: "Error: Test already running");
        
        _testTask = Task.Run(StartTests);
        return new StateResult(State: "Started");
    }


    private void StartTests()
    {
        _sw.Restart();
        
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        using HttpClient httpClient = new(clientHandler);

        object mutex = new();
        long counter = 0;
        
        async Task<IResponse> ExecutionMethod(IScenarioContext context)
        {
            long safeCounter;
            lock (mutex)
            {
                safeCounter = counter++;
            }
            
            var response = await httpClient.GetAsync($"https://{_testHostUrl}/hello/I{safeCounter}");
            
            return response.IsSuccessStatusCode
                ? Response.Ok()
                : Response.Fail();
        }

        var scenario = Scenario.Create("base_scenario", ExecutionMethod)
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(Simulation.RampingInject(rate: 500, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30)))
            .WithMaxFailCount(Int32.MaxValue);


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
            return new StateResult("Tests faulted");
        
        if(_testTask.IsCompleted)
            return new StateResult($"Tests completed in {_sw.Elapsed}");
        
        return new StateResult($"Tests running for {_sw.Elapsed}");
    }
}

public record StateResult(string State);