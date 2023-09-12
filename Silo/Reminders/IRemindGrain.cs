using Orleans.Runtime;

namespace Silo.Reminders;

public interface IRemindGrain : IGrainWithStringKey
{
    
}

public class RemindGrain : Grain, IRemindGrain, IRemindable
{
    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        throw new NotImplementedException();
    }
}