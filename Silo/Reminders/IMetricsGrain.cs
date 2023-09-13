using Abstractions.OrleansCommon;
using Abstractions.Reminders;
using Orleans.Placement;

namespace Silo.Reminders;

public interface IMetricsGrain : IGrainWithIntegerKey
{
    public static IMetricsGrain GetInstance(IGrainFactory gf) => gf.GetGrain<IMetricsGrain>(0);
    ValueTask<MetricsResponse> GetMetrics();
    ValueTask SetValues(double deserializeMs, double reminderMs);
}

[PreferLocalPlacement]
public class MetricsGrain : Grain, IMetricsGrain
{

    private readonly List<double> _deserializeMs = new();
    private readonly List<double> _reminderMs = new();
    
    
    public ValueTask<MetricsResponse> GetMetrics()
    {
        var result = new MetricsResponse(
            LatencyDeserializeState: Latency.FromList(_deserializeMs), 
            LatencyRegisterOrUpdateReminder: Latency.FromList(_reminderMs));

        return ValueTask.FromResult(result);
    }

    
    
    
    public ValueTask SetValues(double deserializeMs, double reminderMs)
    {
        _deserializeMs.Add(deserializeMs);
        _reminderMs.Add(reminderMs);
        
        return ValueTask.CompletedTask;
    }
}

