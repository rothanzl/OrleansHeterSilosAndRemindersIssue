using OrleansDashboard.Model;

namespace Clients.MinimalApi.Bomber.Scenarios;

public class DashboardData
{
    public DateTime LastData { get; private set; }

    private DashboardCounters Counters { get; set; } = new();
    public bool AreObsolete => (DateTime.Now - LastData) >= ObsoleteTimeout;
    private TimeSpan ObsoleteTimeout { get; } = TimeSpan.FromSeconds(10); 
    
    public void Push(DashboardCounters counters)
    {
        LastData = DateTime.Now;
        Counters = counters;
    }
        
}