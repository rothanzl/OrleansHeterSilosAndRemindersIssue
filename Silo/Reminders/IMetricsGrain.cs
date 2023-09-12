namespace Silo.Reminders;

public interface IMetricsGrain : IGrainWithIntegerKey
{
    public static IMetricsGrain GetInstance(IGrainFactory gf) => gf.GetGrain<IMetricsGrain>(0);
    
    

}

