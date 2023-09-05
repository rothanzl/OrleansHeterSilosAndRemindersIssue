using System.Diagnostics;
using Clients.MinimalApi.Bomber.Scenarios;
using NBomber.CSharp;

namespace Clients.MinimalApi.Bomber;

public class Tester
{
    private Task? _testTask;
    private readonly Stopwatch _sw;
    private readonly TesterConfig _config;
    private readonly ILogger<Tester> _logger;

    public Tester(TesterConfig config, ILogger<Tester> logger)
    {
        _config = config;
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

    private async Task StartTests(StartTestRequest req)
    {
        _sw.Restart();

        await using IScenarioMethod scenarioMethod = new SelfLoadingScenario(_config, _logger, req);
        
        var scenario = Scenario.Create("base_scenario", scenarioMethod.Method)
            .WithWarmUpDuration(TimeSpan.FromSeconds(req.WarmingUpDurationSeconds))
            .WithLoadSimulations(Simulation.RampingConstant(copies: req.Concurrency, during: TimeSpan.FromMinutes(req.RampMinutes)))
            .WithLoadSimulations(Simulation.KeepConstant(copies: req.Concurrency, during: TimeSpan.FromMinutes(req.DurationMinutes)))
            .WithMaxFailCount(req.MaxFailedCount);


        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
        
        _sw.Stop();
        
        _logger.LogWarning("Tester ended with {Count} total activated grains in {Time}", scenarioMethod.ActivatedGrains, _sw.Elapsed);
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