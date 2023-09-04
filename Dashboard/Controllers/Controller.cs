using Microsoft.AspNetCore.Mvc;
using OrleansDashboard;
using OrleansDashboard.Model;

namespace Dashboard.Controllers;

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

    [HttpGet]
    public ActionResult<string> Root()
    {
        return Ok("Dashboard");
    }
    
    private IDashboardGrain GetDashboardGrain() => _grainFactory.GetGrain<IDashboardGrain>(0);
    

    [HttpGet("silos")]
    public async Task<ActionResult<SiloDetails[]>> GetSilos()
    {
        var dashboardGrain = GetDashboardGrain();
        var counters = await dashboardGrain.GetCounters();

        var result = counters.Value.Hosts ?? Array.Empty<SiloDetails>();
        return Ok(result);
    }

    [HttpGet("cluster")]
    public async Task<ActionResult<DashboardCounters>> GetCluster()
    {
        var dashboardGrain = GetDashboardGrain();
        var counters = await dashboardGrain.GetCounters();

        return Ok(counters.Value);
    }

    [HttpGet("silo/{address}/tracing")]
    public async Task<ActionResult<Dictionary<string, GrainTraceEntry>>> GetSiloTracing([FromRoute] string address)
    {
        var dashboardGrain = GetDashboardGrain();
        var tracing = await dashboardGrain.GetSiloTracing(address);

        return Ok(tracing.Value);
    }
    
}