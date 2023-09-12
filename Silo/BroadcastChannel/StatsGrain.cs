using Abstractions.BroadcastChannel;

namespace Silo.BroadcastChannel;

public class StatsGrain : Grain, IStatsGrain
{
    
    private readonly ILogger<StatsGrain> _logger;
    
    private Dictionary<long, long> Counters { get; set; } = new();
    private Dictionary<long, List<long[]>> InconsistentCounters { get; set; } = new();
    private List<string> Exceptions { get; set; } = new();
    private long ProducerCounter { get; set; } = 0;

    public StatsGrain(ILogger<StatsGrain> logger)
    {
        _logger = logger;
    }

    public ValueTask<StatsResponse> GetStats()
    {
        var stats = new StatsResponse(ProducerCounter, Counters, InconsistentCounters, Exceptions);
        return ValueTask.FromResult(stats);
    }

    public ValueTask SetStats(Dictionary<long, long> counters, 
        Dictionary<long, List<long[]>> inconsistentCounters,
        List<string> exceptions)
    {
        Counters = counters;
        InconsistentCounters = inconsistentCounters;
        Exceptions = exceptions;
        
        
        return ValueTask.CompletedTask;
    }

    public ValueTask SetProducerCounter(long no)
    {
        ProducerCounter = no;
        return ValueTask.CompletedTask;
    }
}