using NBomber.Contracts;

namespace Clients.MinimalApi.Bomber.Scenarios;

public interface IScenarioMethod : IAsyncDisposable
{
    int ActivatedGrains { get; }
    
    Task<IResponse> Method(IScenarioContext arg);
}