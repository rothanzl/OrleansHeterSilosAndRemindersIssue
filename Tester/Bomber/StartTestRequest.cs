using System.ComponentModel;

namespace Tester.Bomber;

public record StartTestRequest(
    [property: DefaultValue(1)] int Concurrency,
    [property: DefaultValue(5)] double WarmingUpDurationSeconds,
    [property: DefaultValue(0)] double RampMinutes,
    [property: DefaultValue(60)] double DurationMinutes,
    [property: DefaultValue(0)] int ResponseLimitMs,
    [property: DefaultValue(10)] int ExpectedSilos,
    [property:DefaultValue(1)] int MaxFailedCount
);