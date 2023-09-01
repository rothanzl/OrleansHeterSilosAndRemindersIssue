using System.Diagnostics;
using System.Globalization;
using NBomber.Contracts;
using NBomber.CSharp;

namespace Clients.MinimalApi.Bomber;

public class Tester
{
    private Task? _testTask;
    private readonly Stopwatch _sw;
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

        async Task<IResponse> ExecutionMethod(IScenarioContext context)
        {
            long safePrimGrainCounter;
            lock (mutex)
            {
                safePrimGrainCounter = primGrainCounter++;
            }

            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                var response = await httpClient.GetAsync($"https://{_testHostUrl}/hello/I{safePrimGrainCounter}");
                sw.Stop();

                int activatedGrains = 0;
                if (response.IsSuccessStatusCode)
                {
                    var responseBodyString = await response.Content.ReadAsStringAsync();
                    Int32.TryParse(responseBodyString, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out activatedGrains);

                    lock (mutex)
                    {
                        totalActivatedGrains += activatedGrains;
                    }
                }

                _logger.LogInformation("Prim grain {PrimGrain}, activated grains {Count}", safePrimGrainCounter, activatedGrains);

                return response.IsSuccessStatusCode
                    ? Response.Ok(
                        payload: safePrimGrainCounter, 
                        statusCode: response.StatusCode.ToString(), 
                        sizeBytes: activatedGrains)
                    : Response.Fail(
                        statusCode: response.StatusCode.ToString());
            }
            catch (Exception e)
            {
                return Response.Fail(message: e.ToString());
            }
        }

        var scenario = Scenario.Create("base_scenario", ExecutionMethod)
            .WithWarmUpDuration(TimeSpan.FromSeconds(req.WarmingUpDurationSeconds))
            .WithLoadSimulations(Simulation.RampingConstant(copies: req.Concurrency, during: TimeSpan.FromMinutes(req.RampMinutes)))
            .WithLoadSimulations(Simulation.KeepConstant(copies: req.Concurrency, during: TimeSpan.FromMinutes(req.DurationMinutes)))
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