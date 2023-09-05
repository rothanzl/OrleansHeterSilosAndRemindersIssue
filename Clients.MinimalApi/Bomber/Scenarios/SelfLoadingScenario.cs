using System.Diagnostics;
using Abstractions;
using NBomber.Contracts;
using NBomber.CSharp;
using OrleansDashboard.Model;

namespace Clients.MinimalApi.Bomber.Scenarios;

public class SelfLoadingScenario : BaseScenarioMethod
{
    private readonly Task _dashboardTask;
    private bool RunDashboardTask { get; set; }
    private readonly DashboardData _dashboardData;

    public override int ActivatedGrains
    {
        get
        {
            lock (Mutex)
            {
                return _dashboardData.ActivatedGrains;
            }
        }
    }


    public SelfLoadingScenario(TesterConfig config, ILogger logger) : base(config, logger)
    {
        RunDashboardTask = true;
        _dashboardData = new();
        _dashboardTask = Task.Run(DashboardTask);
    }

    private async Task DashboardTask()
    {
        using var httpClient = HttpClientFactory();

        var response = await httpClient.PutAsync($"{_config.TestAppUrl}/populate/start", null);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error start populate with response {Code}", response.StatusCode.ToString());
            return;
        }
        
        try
        {
            Stopwatch sw = new Stopwatch();
            while (RunDashboardTask)
            {
                sw.Restart();

                response = await httpClient.GetAsync($"{_config.DashboardUrl}/cluster");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Dashboard get cluster info response {Code}", response.StatusCode.ToString());
                    return;
                }

                var counters = await response.Content.ReadFromJsonAsync<DashboardCounters>() ??
                               throw new NullReferenceException("Cannot deserialize DashboardCounters");

                lock (Mutex)
                {
                    _dashboardData.Push(counters);
                    _logger.LogInformation("Activated grains {No}", _dashboardData.ActivatedGrains);
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds < 1000)
                    await Task.Delay(1000 - Convert.ToInt32(sw.ElapsedMilliseconds));
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error dashboard task");
        }
        finally
        {
            response = await httpClient.PutAsync($"{_config.TestAppUrl}/populate/stop", null);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error stop populate with response {Code}", response.StatusCode.ToString());
            }
        }
    }
    
    
    public override async Task<IResponse> Method(IScenarioContext context)
    {
        lock (Mutex)
        {
            if(_dashboardData.AreObsolete)
                return Response.Fail(statusCode: "ObsoleteDashboardData");
            
            if(_dashboardData.ChangedNumberOfSilos)
                return Response.Fail(statusCode: "ChangedNumberOfSilos"+_dashboardData.NumberOfSilos);
        }
        
        HttpClient httpClient = GetHttpClient(context);
        Stopwatch sw = Stopwatch.StartNew();
        var response = await httpClient.GetAsync($"{_config.TestAppUrl}/");
        sw.Stop();
        
        if(!response.IsSuccessStatusCode)
            return Response.Fail(statusCode: response.StatusCode.ToString());
        
        if(sw.ElapsedMilliseconds > ClusterConfig.SiloRestResponseDelayLimitMs)
            return Response.Fail(statusCode: "RestApiResponse" + sw.ElapsedMilliseconds);

        return Response.Ok(statusCode: response.StatusCode.ToString());
    }


    public override async ValueTask DisposeAsync()
    {
        RunDashboardTask = false;
        await _dashboardTask;

        await base.DisposeAsync();
    }
}