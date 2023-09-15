using System.Diagnostics;

namespace Silo.Reminders.HealthCheck;

public class RemindersHealthCheckHostedService : IHostedService
{
    private readonly ILogger<RemindersHealthCheckHostedService> _logger;
    private readonly IGrainFactory _grainFactory;

    public RemindersHealthCheckHostedService(ILogger<RemindersHealthCheckHostedService> logger, IGrainFactory grainFactory)
    {
        _logger = logger;
        _grainFactory = grainFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        int counter = 1;
        Stopwatch sw = Stopwatch.StartNew();
        while (true)
        {
            try
            {
                var checkState = await IRemindersHealthCheckGrain.Instance(_grainFactory).Check();

                if (checkState.Healthy)
                {
                    sw.Stop();
                    _logger.LogInformation("Reminders are healthy within {Sec} secs and {Count} iteration, last check {SecCheck} secs", 
                        sw.Elapsed.TotalSeconds.ToString("F1"), 
                        counter, 
                        checkState.Elapsed.TotalSeconds.ToString("F1"));
                    return;
                }
                
                _logger.LogWarning("Reminders are not healthy with error: {Error}", 
                    checkState.Exception?.ToString());

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot get reminders health check");
            }

            await Task.Delay(500, cancellationToken);
            counter += 1;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}