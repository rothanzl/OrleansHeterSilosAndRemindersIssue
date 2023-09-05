using OrleansDashboard.Model;

namespace Clients.MinimalApi.Bomber.Scenarios;

public class DashboardData
{
    public DateTime LastData { get; private set; }

    private DashboardCounters Counters { get; set; } = new();
    public bool AreObsolete => (DateTime.Now - LastData) >= ObsoleteTimeout;
    private TimeSpan ObsoleteTimeout { get; } = TimeSpan.FromSeconds(10);
    public bool ChangedNumberOfSilos { get; private set; } = false;
    public int NumberOfSilos => Counters.Hosts.Length;
    
    
    public bool Push(DashboardCounters counters)
    {
        if (ChangedNumberOfSilos)
            return false;


        ChangedNumberOfSilos = Counters.Hosts.Length != counters.Hosts.Length;
        LastData = DateTime.Now;
        Counters = counters;

        return true;
    }
        
}