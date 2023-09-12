using Microsoft.Extensions.Diagnostics.HealthChecks;
using Silo.Controllers;

namespace Silo.BroadcastChannel;

public class StatsHealthCheck : IHealthCheck
{
    private readonly IGrainFactory _grainFactory;

    public StatsHealthCheck(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var stats = await Controller.GetStats(_grainFactory);
            IReadOnlyDictionary<string, object> data = new Dictionary<string, object>()
            {
                {"stats", stats}
            };
            return HealthCheckResult.Healthy(data: data);
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(exception: e);
        }
    }
}