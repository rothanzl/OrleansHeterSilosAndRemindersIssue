namespace Silo.AutoPopulation;

public interface ITestConfigGrain : IGrainWithIntegerKey
{
    public static ITestConfigGrain GetInstance(IGrainFactory gf) => gf.GetGrain<ITestConfigGrain>(0);

    ValueTask Start();
    ValueTask Stop();
    ValueTask<bool> IsTestEnabled();
}

public class TestConfigGrain : ITestConfigGrain
{
    private bool TestEnabledState { get; set; } = false;
    
    public ValueTask Start()
    {
        TestEnabledState = true;
        return ValueTask.CompletedTask;
    }

    public ValueTask Stop()
    {
        TestEnabledState = false;
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> IsTestEnabled()
    {
        return ValueTask.FromResult(TestEnabledState);
    }
}