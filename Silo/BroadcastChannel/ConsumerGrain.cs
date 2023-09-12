using Orleans.BroadcastChannel;

namespace Silo.BroadcastChannel;

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
        
        
        await GrainFactory.GetGrain<IStatsGrain>(this.GetPrimaryKeyString()).SetStats(_counters, _inconsistentCounters, _exceptions);
    }

    private Task OnError(Exception exception)
    {
        _exceptions.Add(exception.ToString());
        return Task.CompletedTask;
    }

    public ValueTask Activate() => ValueTask.CompletedTask;
}