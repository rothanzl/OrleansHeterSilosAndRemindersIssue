using Abstractions.BroadcastChannel;

namespace Silo.BroadcastChannel;

public interface IStatsGrain : IGrainWithStringKey
{
    ValueTask<StatsResponse> GetStats();
    ValueTask SetStats(Dictionary<long, long> counters, Dictionary<long, List<long[]>> inconsistentCounters, List<string> exceptions);
    ValueTask SetProducerCounter(long no);
}