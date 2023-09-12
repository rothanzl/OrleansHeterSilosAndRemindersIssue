using Abstractions.BroadcastChannel;

namespace Silo.BroadcastChannel;

public interface IStatsGrain : IGrainWithStringKey
{
    ValueTask<StatsResponse> GetStats();
    ValueTask SetStats(StatsResponse stats);
}