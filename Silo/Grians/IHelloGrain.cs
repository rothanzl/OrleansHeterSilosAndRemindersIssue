namespace Silo.Grians;

public interface IHelloGrain : IGrainWithStringKey
{

    Task<int> SayHello(bool recurent);

}