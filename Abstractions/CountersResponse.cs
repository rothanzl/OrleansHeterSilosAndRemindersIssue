using OrleansDashboard.Model;

namespace Abstractions;

public class CountersResponse
{

    public SimpleGrainStatisticCounter[] SimpleGrainStats { get; init; }

    public SiloDetails[] Hosts { get; init; }
    
    public int ActivatedTestGrainCount { get; init; }
    
    public int SystemHosts { get; init; }

}