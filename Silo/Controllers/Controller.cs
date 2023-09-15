using Abstractions;
using Abstractions.Reminders;
using Microsoft.AspNetCore.Mvc;
using Silo.AutoPopulation;
using Silo.Reminders;

namespace Silo.Controllers;


[Route("/")]
public class Controller : ControllerBase
{
    private readonly IGrainFactory _grainFactory;
    private readonly ILogger<Controller> _logger;

    

    public Controller(IGrainFactory grainFactory, ILogger<Controller> logger)
    {
        _grainFactory = grainFactory;
        _logger = logger;
    }


    [HttpGet("/")]
    public async Task<ActionResult<string>> GetIndex()
    {
        await Task.Delay(ClusterConfig.SiloRestResponseDelayMs);

        return Ok("Success answer");
    }

    [HttpPost("test/start")]
    public async Task<ActionResult> StartTest()
    {
        await ITestConfigGrain.GetInstance(_grainFactory).Start();
        return Ok();
    }
    
    [HttpPost("test/stop")]
    public async Task<ActionResult> StopTest()
    {
        await ITestConfigGrain.GetInstance(_grainFactory).Stop();
        return Ok();
    }


    [HttpGet("/reminder/metrics")]
    public async Task<ActionResult<MetricsResponse>> GetReminderMetricsAsync()
    {
        var result = await IMetricsGrain.GetInstance(_grainFactory).GetMetrics();
        return Ok(result);
    }

}