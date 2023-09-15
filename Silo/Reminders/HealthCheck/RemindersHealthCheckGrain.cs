using System.Diagnostics;
using Orleans.Runtime;

namespace Silo.Reminders.HealthCheck;

public class RemindersHealthCheckGrain : Grain, IRemindersHealthCheckGrain, IRemindable
{
    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        return Task.CompletedTask;
    }
    
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    public async Task<IRemindersHealthCheckGrain.State> Check()
    {
        const string name = "test-reminders";
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            var registeredReminder = await this.GetReminder(name);
            if (registeredReminder is { })
            {
                await this.UnregisterReminder(registeredReminder);
            }

            var period = TimeSpan.FromHours(1);
            var newReinder = await this.RegisterOrUpdateReminder(name, period, period);
            await this.UnregisterReminder(newReinder);

            sw.Stop();
            if (sw.Elapsed > Timeout)
            {
                return new IRemindersHealthCheckGrain.State(false, 
                    new TimeoutException($"Passed sequence in {sw.Elapsed.TotalSeconds:F1} sec > {Timeout.TotalSeconds:F1}"),
                    sw.Elapsed);
            }
        }
        catch (Exception e)
        {
            sw.Stop();
            return new IRemindersHealthCheckGrain.State(false, e, sw.Elapsed);
        }

        return new IRemindersHealthCheckGrain.State(true, null, sw.Elapsed);
    }
}