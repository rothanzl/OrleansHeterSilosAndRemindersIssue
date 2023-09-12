namespace Abstractions.BroadcastChannel;

[GenerateSerializer]
public record StatsResponse(
    long ProducerCounter,
    Dictionary<long, long> Counters,
    Dictionary<long, List<long[]>> InconsistentCounters,
    List<string> Exceptions)
{
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);

    public static StatsResponse Empty { get; } = new(default, new(), new(), new());
}