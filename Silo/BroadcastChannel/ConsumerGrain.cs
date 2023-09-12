using Abstractions.BroadcastChannel;
using Orleans.BroadcastChannel;

namespace Silo.BroadcastChannel;

public interface IStatsGrain : IGrainWithStringKey
{
    ValueTask<StatsResponse> GetStats();
    ValueTask SetStats(StatsResponse stats);
}

public class StatsGrain : Grain, IStatsGrain
{
    private StatsResponse Stats { get; set; } = StatsResponse.Empty;

    public ValueTask<StatsResponse> GetStats()
    {
        return ValueTask.FromResult(Stats);
    }

    public ValueTask SetStats(StatsResponse stats)
    {
        Stats = stats;
        return ValueTask.CompletedTask;
    }
}


public interface IConsumerGrain : IGrainWithStringKey
{
    ValueTask Activate();
}

[ImplicitChannelSubscription]
public class ConsumerGrain : Grain, IConsumerGrain, IOnBroadcastChannelSubscribed
{
    private readonly Dictionary<long, long> _counters = new();


    private readonly Dictionary<long, List<long[]>> _inconsistentCounters = new();
    private readonly List<string> _exceptions = new();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var pk = this.GetPrimaryKeyString();
        
        return base.OnActivateAsync(cancellationToken);
    }


    public Task OnSubscribed(IBroadcastChannelSubscription streamSubscription) =>
        streamSubscription.Attach<long[]>(OnUpdated, OnError);
    

    private async Task OnUpdated(long[] payload)
    {
        var generation = payload[0];
        var counter = payload[1];

        await Task.Delay(1000);
        
        if (_counters.TryGetValue(generation, out var prevCounter))
        {
            if (prevCounter + 1 != counter)
            {
                if (!_inconsistentCounters.ContainsKey(generation))
                    _inconsistentCounters[generation] = new();

                _inconsistentCounters[generation].Add(new long[] {prevCounter, counter});
            }
        }
        
        _counters[generation] = counter;
        
        
        // Publish stats
        var stats = new StatsResponse(_counters, _inconsistentCounters, _exceptions);
        await GrainFactory.GetGrain<IStatsGrain>(this.GetPrimaryKeyString()).SetStats(stats);
    }

    private Task OnError(Exception exception)
    {
        _exceptions.Add(exception.ToString());
        return Task.CompletedTask;
    }

    public ValueTask Activate() => ValueTask.CompletedTask;
}