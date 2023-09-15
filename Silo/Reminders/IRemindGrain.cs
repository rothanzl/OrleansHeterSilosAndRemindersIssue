using System.Diagnostics;
using Orleans.Runtime;

namespace Silo.Reminders;

public interface IRemindGrain : IGrainWithStringKey
{
    ValueTask Init();
}


public class RemindGrain : Grain, IRemindGrain, IRemindable
{
    private readonly ILogger<RemindGrain> _logger;

    public RemindGrain(ILogger<RemindGrain> logger)
    {
        _logger = logger;
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        return Task.Delay(2000);
    }

    public async ValueTask Init()
    {
        Stopwatch sw = Stopwatch.StartNew();
        
        var msDeserialize = sw.ElapsedMilliseconds;
        sw.Restart();
        
        await this.RegisterOrUpdateReminder("shipmentState", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        var msRegisterOrUpdateReminder = sw.ElapsedMilliseconds;
        
        _logger.LogInformation("Init deserialize {D}ms, reminder {R}ms", 
            msDeserialize.ToString(), msRegisterOrUpdateReminder.ToString());

        await IMetricsGrain.GetInstance(GrainFactory).SetValues(deserializeMs: msDeserialize, reminderMs: msRegisterOrUpdateReminder);
    }
}