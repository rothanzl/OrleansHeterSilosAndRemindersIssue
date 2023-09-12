using System.Diagnostics;
using Abstractions.OrleansCommon;
using Orleans.Placement;
using Silo.AutoPopulation;

namespace Silo.TestGrains;


[DontPlaceMeOnTheDashboard]
[PreferLocalPlacement]
public class RecurrentTestGrainInMemory : Grain, IRecurrentTestGrainInMemory
{
    private long[] State { get; set; }


    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        State = PopulationService.CreatePayload(0, 0);
        DelayDeactivation(TimeSpan.FromHours(24));

    }


    public async Task<int> SayHello(bool recurent)
    {
        if (!recurent)
            return 1;
        
        
        var pk = this.GetPrimaryKeyString()!;

        int i = 0;
        Stopwatch sw = Stopwatch.StartNew();
        
        while (true)
        {
            if (sw.ElapsedMilliseconds >= 1000)
                break;
            
            string subGrainId = pk + "-sg" + (++i);
            await GrainFactory.GetGrain<IRecurrentTestGrainInMemory>(subGrainId).SayHello(recurent: false);
        }
        
        return i + 1;
    }
}