using System.ComponentModel;

namespace Clients.MinimalApi.Bomber;

public record StartTestRequest(
    [property:DefaultValue(10)] int Concurrency, 
    [property:DefaultValue(5)] double WarmingUpDurationSeconds,
    [property:DefaultValue(0)] double RampMinutes, 
    [property:DefaultValue(60)] double DurationMinutes,
    [property:DefaultValue(500)] int ResponseLimitMs,
    [property:DefaultValue(100)] int MaxFailedCount
);