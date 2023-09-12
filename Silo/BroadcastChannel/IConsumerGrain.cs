namespace Silo.BroadcastChannel;

public interface IConsumerGrain : IGrainWithStringKey
{
    ValueTask Activate();
}