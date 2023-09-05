using NBomber.Contracts;

namespace Clients.MinimalApi.Bomber.Scenarios;

public abstract class BaseScenarioMethod : IScenarioMethod
{
    protected object Mutex { get; } = new();
    public int ActivatedGrains { get; protected set; }
    private List<HttpClient> HttpClients { get; } = new();
    protected readonly TesterConfig _config;
    protected readonly ILogger _logger;

    protected BaseScenarioMethod(TesterConfig config, ILogger logger)
    {
        _config = config;
        _logger = logger;
    }
    
    public virtual ValueTask DisposeAsync()
    {
        foreach (HttpClient httpClient in HttpClients)
        {
            httpClient.Dispose();
        }
        
        return ValueTask.CompletedTask;
    }
    
    protected static HttpClient HttpClientFactory()
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        HttpClient httpClient = new(clientHandler);
        return httpClient;
    }

    protected HttpClient GetHttpClient(IScenarioContext context)
    {
        if (context.Data.TryGetValue("http-client", out var value))
        {
            return (HttpClient) value;
        }
        
        HttpClient httpClient = HttpClientFactory();
        context.Data["http-client"] = httpClient;
        lock (Mutex)
        {
            HttpClients.Add(httpClient);
        }

        return httpClient;
    }

    public abstract Task<IResponse> Method(IScenarioContext context);
}