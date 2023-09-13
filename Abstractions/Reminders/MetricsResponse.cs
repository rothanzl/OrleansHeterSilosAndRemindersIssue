namespace Abstractions.Reminders;

[GenerateSerializer]
public record MetricsResponse(Latency LatencyDeserializeState, Latency LatencyRegisterOrUpdateReminder);

[GenerateSerializer]
public record Latency(double Average, double Last, int Count)
{

    public static Latency FromList(IReadOnlyList<double> source)
    {
        if (source.Count == 0)
            return new Latency(0, 0, 0);

        return new Latency(
            Average: source.Average(),
            Last: source.Last(),
            Count: source.Count);
    }
    
}