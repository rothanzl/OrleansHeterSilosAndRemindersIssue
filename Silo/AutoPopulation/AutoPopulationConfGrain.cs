namespace Silo.AutoPopulation;

public interface IAutoPopulationConfGrain : IGrainWithIntegerKey
{
    public static IAutoPopulationConfGrain GetInstance(IGrainFactory gf) => gf.GetGrain<IAutoPopulationConfGrain>(0);

    ValueTask StartPopulation();
    ValueTask StopPopulation();
    ValueTask<bool> IsPopulationEnabled();
}

public class AutoPopulationConfGrain : IAutoPopulationConfGrain
{
    private bool PopulationEnabledState { get; set; } = false;
    
    public ValueTask StartPopulation()
    {
        PopulationEnabledState = true;
        return ValueTask.CompletedTask;
    }

    public ValueTask StopPopulation()
    {
        PopulationEnabledState = false;
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> IsPopulationEnabled()
    {
        return ValueTask.FromResult(PopulationEnabledState);
    }
}