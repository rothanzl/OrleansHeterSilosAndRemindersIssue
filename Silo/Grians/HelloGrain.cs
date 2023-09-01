using System.Collections.Immutable;
using System.Diagnostics;
using Common.Orleans;
using Foundation.ObjectHydrator;
using Silo.Grians.States;

namespace Silo.Grians;


[DontPlaceMeOnTheDashboard]
public class HelloGrain : Grain, IHelloGrain
{
    private DemoState State { get; set; } = new();
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var hydrator = new Hydrator<DemoState>()
                .With(x=>x.Address, new Hydrator<DemoState.LockerAddress>())
                .With(x => x.LastTelemeries, new Hydrator<LockerTelemetryDto>()
                    .With(x=> x.ShutdownReasons, new Hydrator<LockerShutdownReasonType>().GetList(10).ToImmutableList())
                    .GetList(10))
                .With(x=>x.LockerHealthCheck, new Hydrator<LockerHealthCheckOut>())
                .With(x => x.SigningKeys, new Hydrator<DemoState.LockerSigningKeys>())
                .With(x => x.Alerts, new Hydrator<DemoState.LockerActiveAlert>().GetList(10))
                .With(x => x.LockerCommands, new Hydrator<Guid>().GetList(10))
                .With(x => x.Inventories, new Hydrator<DemoState.LockerInventoryConnector>().GetList(10))
                .With(x => x.Incidents, new Hydrator<DemoState.LockerActiveIncident>().GetList(10))
                .With(x => x.MalfunctionSlots, new Hydrator<DemoState.SlotAggregation>()
                    .With(x => x.SlotIndexes, new Hydrator<byte>().GetList(10)))
                .With(x => x.InventorySlots, new Hydrator<DemoState.SlotAggregation>()
                    .With(x => x.SlotIndexes, new Hydrator<byte>().GetList(10)))
            ;
        
        State = hydrator.Generate();
        
        DelayDeactivation(TimeSpan.FromMinutes(60));
        
        return base.OnActivateAsync(cancellationToken);
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
            await GrainFactory.GetGrain<IHelloGrain>(subGrainId).SayHello(recurent: false);
        }
        
        return i + 1;
    }
}