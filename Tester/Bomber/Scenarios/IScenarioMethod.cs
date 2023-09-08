using NBomber.Contracts;

namespace Tester.Bomber.Scenarios;

public interface IScenarioMethod : IAsyncDisposable
{
    Task Init();

    Task<IResponse> Method(IScenarioContext arg);
    void TestEndHook(TimeSpan testDuration);
}