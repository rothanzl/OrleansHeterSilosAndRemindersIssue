namespace Abstractions.Reminders;

[GenerateSerializer]
public record MetricsResponse(Latency DeserializeState, Latency RegisterOrUpdateReminder);

[GenerateSerializer]
public record Latency(double Average, double Last);