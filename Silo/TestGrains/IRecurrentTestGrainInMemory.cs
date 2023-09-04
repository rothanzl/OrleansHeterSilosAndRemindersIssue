namespace Silo.TestGrains;

public interface IRecurrentTestGrainInMemory : IGrainWithStringKey
{

    Task<int> SayHello(bool recurent);

}