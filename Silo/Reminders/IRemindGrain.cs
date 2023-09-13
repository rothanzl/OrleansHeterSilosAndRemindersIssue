using System.Diagnostics;
using Newtonsoft.Json;
using Orleans.Runtime;
using Silo.TestGrains.States;
using Abstractions.OrleansCommon;
using Orleans.Placement;

namespace Silo.Reminders;

public interface IRemindGrain : IGrainWithStringKey
{
    ValueTask Init();
}

[DontPlaceMeOnTheDashboard]
[PreferLocalPlacement]
public class RemindGrain : Grain, IRemindGrain, IRemindable
{
    private ShipmentDemoState State { get; set; } = new();
    private readonly ILogger<RemindGrain> _logger;

    public RemindGrain(ILogger<RemindGrain> logger)
    {
        _logger = logger;
    }

    public Task ReceiveReminder(string reminderName, TickStatus status)
    {
        return Task.Delay(2000);
    }

    public async ValueTask Init()
    {
            Stopwatch sw = Stopwatch.StartNew();
        
        string json = @"{
            'Id': 123456789,
            'TenantId': '00000000-0000-0000-0000-000000000000',
            'SlotId': 123456789,
            'SlotIndex': 255,
            'CabinetIndex': 255,
            'CabinetId': 123456789,
            'LockerId': 123456789,
            'DroppedSlotId': 123456789,
            'State': 'ShipmentStateLegacyValue',
            'StateName': 'ShipmentStateValue',
            'Width': 32767,
            'Height': 32767,
            'Depth': 32767,
            'Weight': 2147483647,
            'Created': '2023-09-12T12:34:56.789Z',
            'PickUpTo': '2023-09-12T23:59:59.999Z',
            'Dropped': '2023-09-12T12:34:56.789Z',
            'Canceled': '2023-09-12T12:34:56.789Z',
            'Returned': '2023-09-12T12:34:56.789Z',
            'PickedUp': '2023-09-12T12:34:56.789Z',
            'PickUpCode': 'PickUpCodeValue',
            'DropOffCode': 'DropOffCodeValue',
            'DropOffRole': 'OperationRoleValue',
            'DropOffByActor': 'DropOffByActorValue',
            'DropOffByDeviceId': 'DropOffByDeviceIdValue',
            'DropOffCommitedByActor': 'DropOffCommitedByActorValue',
            'DropOffCommited': '2023-09-12T12:34:56.789Z',
            'DropOffCommitedByDeviceId': 'DropOffCommitedByDeviceIdValue',
            'DropOffCommitedByRole': 'OperationRoleValue',
            'PickUpRole': 'OperationRoleValue',
            'PickupCommited': '2023-09-12T12:34:56.789Z',
            'PickUpByActor': 'PickUpByActorValue',
            'PickUpByDeviceId': 'PickUpByDeviceIdValue',
            'PickUpCommitedByActor': 'PickUpCommitedByActorValue',
            'PickUpCommitedByDeviceId': 'PickUpCommitedByDeviceIdValue',
            'PickUpCommitedByRole': 'OperationRoleValue',
            'SenderExternalId': 'SenderExternalIdValue',
            'RecipientExternalId': 'RecipientExternalIdValue',
            'NumberOfPackages': 123456789,
            'ExternalId': 'ExternalIdValue',
            'DestinationLockerId': 123456789,
            'DestinationLockerExternalId': 'DestinationLockerExternalIdValue'
        }";

        State = JsonConvert.DeserializeObject<ShipmentDemoState>(json)!;
        var msDeserialize = sw.ElapsedMilliseconds;
        sw.Restart();
        
        await this.RegisterOrUpdateReminder("shipmentState", TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
        var msRegisterOrUpdateReminder = sw.ElapsedMilliseconds;
        
        _logger.LogInformation("Init deserialize {D}ms, reminder {R}ms", 
            msDeserialize.ToString(), msRegisterOrUpdateReminder.ToString());

        await IMetricsGrain.GetInstance(GrainFactory).SetValues(deserializeMs: msDeserialize, reminderMs: msRegisterOrUpdateReminder);
    }
}