using System.Collections.Immutable;
using System.Diagnostics;
using Abstractions.OrleansCommon;
using Foundation.ObjectHydrator;
using Orleans.Placement;
using Orleans.Runtime;
using Silo.TestGrains.States;

namespace Silo.TestGrains;


[DontPlaceMeOnTheDashboard]
[PreferLocalPlacement]
public class RecurrentTestGrainInMemory : Grain, IRecurrentTestGrainInMemory
{
    private readonly IPersistentState<DemoState> _persistentState;

    public RecurrentTestGrainInMemory([PersistentState(stateName: "demo-state")] IPersistentState<DemoState> persistentState)
    {
        _persistentState = persistentState;
    }

    
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
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
        
        _persistentState.State = hydrator.Generate();

        DelayDeactivation(TimeSpan.FromHours(24));

        await _persistentState.WriteStateAsync();
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