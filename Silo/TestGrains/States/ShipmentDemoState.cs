namespace Silo.TestGrains.States
{
    public class ShipmentDemoState
    {
        public int Id { get; set; }
        public Guid TenantId { get; set; }
        public int? SlotId { get; set; }
        public byte? SlotIndex { get; set; }
        public byte? CabinetIndex { get; set; }
        public int? CabinetId { get; set; }
        public int? LockerId { get; set; }
        public int? DroppedSlotId { get; set; }
        public string State { get; set; }
        public string StateName { get; set; }
        public short? Width { get; set; }
        public short? Height { get; set; }
        public short? Depth { get; set; }
        public int? Weight { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset PickUpTo { get; set; }
        public DateTimeOffset? Dropped { get; set; }
        public DateTimeOffset? Canceled { get; set; }
        public DateTimeOffset? Returned { get; set; }
        public DateTimeOffset? PickedUp { get; set; }
        public string PickUpCode { get; set; }
        public string DropOffCode { get; set; }
        public string DropOffRole { get; set; }
        public string DropOffByActor { get; set; }
        public string DropOffByDeviceId { get; set; }
        public string DropOffCommitedByActor { get; set; }
        public DateTimeOffset? DropOffCommited { get; set; }
        public string DropOffCommitedByDeviceId { get; set; }
        public string DropOffCommitedByRole { get; set; }
        public string PickUpRole { get; set; }
        public DateTimeOffset? PickupCommited { get; set; }
        public string PickUpByActor { get; set; }
        public string PickUpByDeviceId { get; set; }
        public string PickUpCommitedByActor { get; set; }
        public string PickUpCommitedByDeviceId { get; set; }
        public string PickUpCommitedByRole { get; set; }
        public string SenderExternalId { get; set; }
        public string RecipientExternalId { get; set; }
        public int NumberOfPackages { get; set; }
        public string ExternalId { get; set; }
        public int? DestinationLockerId { get; set; }
        public string DestinationLockerExternalId { get; set; }
    }
}
