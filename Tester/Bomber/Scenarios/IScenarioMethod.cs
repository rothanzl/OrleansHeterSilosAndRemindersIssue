using NBomber.Contracts;

namespace Tester.Bomber.Scenarios;

public interface IScenarioMethod : IAsyncDisposable
{
    int ActivatedGrains { get; }

    Task Init();

    Task<IResponse> Method(IScenarioContext arg);
}