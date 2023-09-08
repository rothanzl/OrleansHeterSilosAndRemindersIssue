namespace Abstractions.BroadcastChannel;

[GenerateSerializer]
public record StatsResponse(
    Dictionary<long, long> Counters, 
    Dictionary<long, List<Tuple<long, long>>> InconsistentCounters,
    List<string> Exceptions);