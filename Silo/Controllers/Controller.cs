using Abstractions;
using Microsoft.AspNetCore.Mvc;
using Silo.AutoPopulation;
using Silo.TestGrains;

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


    [HttpGet("hello/{name}")]
    public async Task<ActionResult<string>> SayHello([FromRoute] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Name is empty!";

        int count;
        try
        {
            count = await _grainFactory.GetGrain<IRecurrentTestGrainInMemory>(name).SayHello(recurent: true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SayHello error");
            throw;
        }

        return Ok(count.ToString());
    }

    [HttpPut("populate/start")]
    public async Task<ActionResult> StartPopulate()
    {
        await IAutoPopulationConfGrain.GetInstance(_grainFactory).StartPopulation();
        return Ok();
    }
    
    [HttpPut("populate/stop")]
    public async Task<ActionResult> StopPopulate()
    {
        await IAutoPopulationConfGrain.GetInstance(_grainFactory).StopPopulation();
        return Ok();
    }
}