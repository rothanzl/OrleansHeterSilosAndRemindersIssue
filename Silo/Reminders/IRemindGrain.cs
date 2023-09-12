using Orleans.Runtime;

namespace Silo.Reminders;

public interface IRemindGrain : IGrainWithStringKey
{
    ValueTask Init();
}

public class RemindGrain : Grain, IRemindGrain, IRemindable
{
    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        throw new NotImplementedException();
    }

    public ValueTask Init()
    {
        throw new NotImplementedException();
    }
}