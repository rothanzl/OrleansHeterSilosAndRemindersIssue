using Abstractions.BroadcastChannel;

namespace Silo.BroadcastChannel;

public class StatsGrain : Grain, IStatsGrain
{
    private StatsResponse Stats { get; set; } = StatsResponse.Empty;
    private readonly ILogger<StatsGrain> _logger;

    public StatsGrain(ILogger<StatsGrain> logger)
    {
        _logger = logger;
    }

    public ValueTask<StatsResponse> GetStats()
    {
        return ValueTask.FromResult(Stats);
    }

    public ValueTask SetStats(StatsResponse stats)
    {
        Stats = stats;
        
        _logger.LogInformation("Got stats: {S}", Stats.ToString());
        
        return ValueTask.CompletedTask;
    }
}