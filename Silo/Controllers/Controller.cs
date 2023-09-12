using Abstractions;
using Abstractions.BroadcastChannel;
using Microsoft.AspNetCore.Mvc;
using Orleans.Runtime;
using OrleansDashboard;
using OrleansDashboard.Metrics.TypeFormatting;
using OrleansDashboard.Model;
using Silo.AutoPopulation;
using Silo.BroadcastChannel;
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

    [HttpPost("test/start")]
    public async Task<ActionResult> StartPopulate()
    {
        await ITestConfigGrain.GetInstance(_grainFactory).Start();
        return Ok();
    }
    
    [HttpPost("test/stop")]
    public async Task<ActionResult> StopPopulate()
    {
        await ITestConfigGrain.GetInstance(_grainFactory).Stop();
        return Ok();
    }

    [HttpGet("broadcast")]
    public async Task<ActionResult<StatsResponse>> GetBroadcastStats()
    {
        var stats = await GetStats(_grainFactory);
        return Ok(stats);
    }

    public static ValueTask<StatsResponse> GetStats(IGrainFactory grainFactory) => grainFactory.GetGrain<IStatsGrain>(Constants.Key).GetStats();


    [HttpGet("counters")]
    public async Task<ActionResult<CountersResponse>> GetCounters()
    {
        var metricsGrain = _grainFactory.GetGrain<IManagementGrain>(0);
        var simpleGrainStats = await  metricsGrain.GetSimpleGrainStatistics();
        var siloDetails = await metricsGrain.GetDetailedHosts(true);

        var hosts = siloDetails.Select(x => new SiloDetails
        {
            FaultZone = x.FaultZone,
            HostName = x.HostName,
            IAmAliveTime = x.IAmAliveTime.ToISOString(),
            ProxyPort = x.ProxyPort,
            RoleName = x.RoleName,
            SiloAddress = x.SiloAddress.ToParsableString(),
            SiloName = x.SiloName,
            StartTime = x.StartTime.ToISOString(),
            Status = x.Status.ToString(),
            SiloStatus = x.Status,
            UpdateZone = x.UpdateZone
        }).ToArray();

        var simpleGrainStatsCounters = simpleGrainStats.Select(x =>
        {
            var grainName = TypeFormatter.Parse(x.GrainType);
            var siloAddress = x.SiloAddress.ToParsableString();

            var result = new SimpleGrainStatisticCounter
            {
                ActivationCount = x.ActivationCount,
                GrainType = grainName,
                SiloAddress = siloAddress,
                TotalSeconds = default,
            };

            return result;
        }).ToArray();


        return Ok(new CountersResponse()
        {
            SimpleGrainStats = simpleGrainStatsCounters,
            Hosts = hosts,
            ActivatedTestGrainCount = simpleGrainStatsCounters
                .Where(g=> g.GrainType.Equals(typeof(RecurrentTestGrainInMemory).FullName))
                .Sum(g=>g.ActivationCount),
            SystemHosts = hosts
                .Count(h => h.SiloName.Equals(ClusterConfig.SiloName))
        });
    }

}