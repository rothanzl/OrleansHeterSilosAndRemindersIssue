namespace Silo.Reminders.HealthCheck;

public interface IRemindersHealthCheckGrain : IGrainWithStringKey
{
    public static IRemindersHealthCheckGrain  Instance(IGrainFactory gf) => gf.GetGrain<IRemindersHealthCheckGrain>("default");

    Task<State> Check();

    [GenerateSerializer]
    public record State(bool Healthy, Exception? Exception, TimeSpan Elapsed);

}