namespace Silo.Grians;

public class HelloGrain : Grain, IHelloGrain
{
    public Task<string> SayHello()
    {
        string message = $"Hello from {this.GetPrimaryKeyString()}";

        return Task.FromResult(message);
    }
}