namespace Clients.MinimalApi.Bomber;

public class TesterConfig
{
    public TesterConfig(ConfigurationManager configurationManager)
    {
        TestAppUrl = configurationManager.GetValue<string>("TestAppUrl") ?? "localhost:5000";
        DashboardUrl = configurationManager.GetValue<string>("DashboardUrl") ?? "localhost:5001";
    }
    
    public string TestAppUrl { get; }
    public string DashboardUrl { get; }
    
}