namespace Silo.Grians;

public interface IHelloGrain : IGrainWithStringKey
{

    Task<string> SayHello(int subgrainsCount);

}