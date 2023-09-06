using NBomber.Contracts;

namespace Clients.MinimalApi.Bomber.Scenarios;

public abstract class BaseScenarioMethod : IScenarioMethod
{
    protected object Mutex { get; } = new();
    public virtual int ActivatedGrains { get; protected set; }
    protected readonly TesterConfig _config;
    protected readonly ILogger _logger;

    protected BaseScenarioMethod(TesterConfig config, ILogger logger)
    {
        _config = config;
        _logger = logger;
    }
    
    public virtual Task Init() => Task.CompletedTask;

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
    
    protected static HttpClient HttpClientFactory()
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        HttpClient httpClient = new(clientHandler);
        return httpClient;
    }

    public abstract Task<IResponse> Method(IScenarioContext context);
}