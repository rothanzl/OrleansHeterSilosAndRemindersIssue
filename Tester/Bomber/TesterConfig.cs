namespace Tester.Bomber;

public class TesterConfig
{
    public TesterConfig(ConfigurationManager configurationManager)
    {
        var testAppUrlEnvVar = configurationManager.GetValue<string>("TestAppUrl");
        var dashboardUrlEvnVar = configurationManager.GetValue<string>("DashboardUrl");
        
        TestAppUrl = testAppUrlEnvVar is null ? "https://localhost:5000" : "https://"+testAppUrlEnvVar;
        DashboardUrl = dashboardUrlEvnVar is null ? "http://localhost:5001" : "https://"+dashboardUrlEvnVar;
    }
    
    public string TestAppUrl { get; }
    public string DashboardUrl { get; }
    
}