using System.Collections.Immutable;
using Newtonsoft.Json;

namespace Silo.TestGrains.States;

public class DemoState
{
    public class SlotAggregation
    {
        public int SlotsAffectedCount { get; set; }

        public List<byte> SlotIndexes { get; set; } = new();
    }

    public class LockerInventoryConnector
    {
        public Guid InventoryId { get; set; }
        public DateTimeOffset Received { get; set; }
    }
    
    public class LockerChangeInventory
    {
        public DateTimeOffset LastChange { get; set; }

        public bool NotifiactionSended { get; set; }
    }
    
    public class LockerActiveAlert
    {
        public Guid AlertId { get; set; }

        public string AlertType { get; set; }
    }

    public class LockerActiveIncident
    {
        public Guid IncidentId { get; set; }

        public string IncidentType { get; set; }
    }


    [Serializable]
    public class LockerSigningKeys
    {
        public string PickupSigningKey { get; set; }
        public string DropOffSigningKey { get; set; }
        public string ServiceSigningKey { get; set; }
    }

    [Serializable]
    public class LockerAddress
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

        public string Notes { get; set; }

        // Point is not serializable to JSON, so we have to store X,Y
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public int Id { get; set; }
    public string FriendlyName { get; set; }
    public string Name { get; set; }

    public LockerAddress Address { get; set; } = new();
    public bool InOperation { get; set; }
    public string Sim { get; set; }
    public string HwMcu { get; set; }
    public LockerSigningKeys SigningKeys { get; set; } = new();

    public long Version { get; set; } = 0;
    public int? BatteryCount { get; set; }
    public int? SolarCount { get; set; }

    
    public string State { get; set; }
    public string AssetState { get; set; }
    public bool? HasExternalPower { get; set; }
    public string ServiceNote { get; set; }
    public bool AllowConfiguration { get; set; } = false;
    public string Cfg { get; set; }
    public bool UseNewConfiguration { get; set; } = false;
    
    public string McuVersion { get; set; }

    public List<Guid> LockerCommands { get; set; } = new();
    public Guid TenantId { get; set; }
    public int FirmwareId { get; set; }

    public List<LockerInventoryConnector> Inventories { get; set; } = new();

    public Guid LastLockerInventory { get; set; }

    public List<LockerActiveAlert> Alerts { get; set; }

    public List<LockerActiveIncident> Incidents { get; set; }

    public List<LockerTelemetryDto> LastTelemeries { get; set; }

    public DateTimeOffset? LastCommunication { get; set; }

    public LockerHealthCheckOut LockerHealthCheck { get; set; }

    public object LockerPerimeters { get; set; }
    public SlotAggregation MalfunctionSlots { get; set; }

    public SlotAggregation InventorySlots { get; set; }
}

public class LockerHealthCheckOut
{
    [Id(0)]
    public LockerHeathStatus Status { get; set; }
    // [Id(1)]
    // public LockerHealthCommunication Communication { get; set; }
    //
    // [Id(2)]
    // public LockerHealthCabinets Cabinets { get; set; }
    //
    // [Id(3)]
    // public LockerHealthBattery Battery { get; set; }
    //
    // [Id(4)]
    // public LockerHealthSlots Slots { get; set; }
    //
    // [Id(5)]
    // public LockerHealthState LockerState { get; set; }

    [JsonProperty("reason-1")]
    [Id(6)]
    public LockerHealthShutdownReason LockerHealthShutdownReason1 { get; set; }

    [JsonProperty("reason-2")]
    [Id(7)]
    public LockerHealthShutdownReason LockerHealthShutdownReason2 { get; set; }

    [JsonProperty("reason-3")]
    [Id(8)]
    public LockerHealthShutdownReason LockerHealthShutdownReason3 { get; set; }

    [JsonProperty("reason-4")]
    [Id(9)]
    public LockerHealthShutdownReason LockerHealthShutdownReason4 { get; set; }

    [JsonProperty("reason-5")]
    [Id(10)]
    public LockerHealthShutdownReason LockerHealthShutdownReason5 { get; set; }

    [JsonProperty("reason-6")]
    [Id(11)]
    public LockerHealthShutdownReason LockerHealthShutdownReason6 { get; set; }

    [JsonProperty("reason-7")]
    [Id(12)]
    public LockerHealthShutdownReason LockerHealthShutdownReason7 { get; set; }

    [JsonProperty("reason-8")]
    [Id(13)]
    public LockerHealthShutdownReason LockerHealthShutdownReason8 { get; set; }

    [JsonProperty("reason-9")]
    [Id(14)]
    public LockerHealthShutdownReason LockerHealthShutdownReason9 { get; set; }

    [JsonProperty("reason-10")]
    [Id(15)]
    public LockerHealthShutdownReason LockerHealthShutdownReason10 { get; set; }

    [JsonProperty("reason-11")]
    [Id(16)]
    public LockerHealthShutdownReason LockerHealthShutdownReason11 { get; set; }

    [JsonProperty("reason-12")]
    [Id(17)]
    public LockerHealthShutdownReason LockerHealthShutdownReason12 { get; set; }

    [JsonProperty("reason-13")]
    [Id(18)]
    public LockerHealthShutdownReason LockerHealthShutdownReason13 { get; set; }

    [JsonProperty("reason-14")]
    [Id(19)]
    public LockerHealthShutdownReason LockerHealthShutdownReason14 { get; set; }

    [JsonProperty("reason-15")]
    [Id(20)]
    public LockerHealthShutdownReason LockerHealthShutdownReason15 { get; set; }

    [JsonProperty("reason-16")]
    [Id(21)]
    public LockerHealthShutdownReason LockerHealthShutdownReason16 { get; set; }

    [JsonProperty("reason-17")]
    [Id(22)]
    public LockerHealthShutdownReason LockerHealthShutdownReason17 { get; set; }

    [JsonProperty("reason-18")]
    [Id(23)]
    public LockerHealthShutdownReason LockerHealthShutdownReason18 { get; set; }

    [JsonProperty("reason-19")]
    [Id(24)]
    public LockerHealthShutdownReason LockerHealthShutdownReason19 { get; set; }

    [JsonProperty("reason-20")]
    [Id(25)]
    public LockerHealthShutdownReason LockerHealthShutdownReason20 { get; set; }

    [JsonProperty("reason-21")]
    [Id(26)]
    public LockerHealthShutdownReason LockerHealthShutdownReason21 { get; set; }

    [JsonProperty("reason-22")]
    [Id(27)]
    public LockerHealthShutdownReason LockerHealthShutdownReason22 { get; set; }

    [JsonProperty("reason-23")]
    [Id(28)]
    public LockerHealthShutdownReason LockerHealthShutdownReason23 { get; set; }

    [JsonProperty("reason-24")]
    [Id(29)]
    public LockerHealthShutdownReason LockerHealthShutdownReason24 { get; set; }

    [JsonProperty("reason-25")]
    [Id(30)]
    public LockerHealthShutdownReason LockerHealthShutdownReason25 { get; set; }

    [JsonProperty("reason-26")]
    [Id(31)]
    public LockerHealthShutdownReason LockerHealthShutdownReason26 { get; set; }

    [JsonProperty("reason-27")]
    [Id(32)]
    public LockerHealthShutdownReason LockerHealthShutdownReason27 { get; set; }

    [JsonProperty("reason-28")]
    [Id(33)]
    public LockerHealthShutdownReason LockerHealthShutdownReason28 { get; set; }

    [JsonProperty("reason-29")]
    [Id(34)]
    public LockerHealthShutdownReason LockerHealthShutdownReason29 { get; set; }

    [JsonProperty("reason-30")]
    [Id(35)]
    public LockerHealthShutdownReason LockerHealthShutdownReason30 { get; set; }

    [JsonProperty("reason-31")]
    [Id(36)]
    public LockerHealthShutdownReason LockerHealthShutdownReason31 { get; set; }
}

public class LockerHealthShutdownReason
{
    [Id(0)]
    public LockerHeathStatus Status { get; set; }
    [Id(1)]
    public string Description { get; set; }
}

public enum LockerHeathStatus
{
    Unhealthy = 0,
    Degraded = 1,
    Healthy = 2
}

public class LockerTelemetryDto
{
    [Id(0)]
    public DateTimeOffset Received { get; init; }

    [Id(1)]
    public int LockerId { get; init; }

    [Id(2)]
    public string Fw { get; init; }

    [Id(3)]
    public string Hw { get; init; }

    [Id(4)]
    public string Part { get; init; }

    [Id(5)]
    public int? Ram { get; init; }

    [Id(6)]
    public int? Flash { get; init; }

    [Id(7)]
    public string Sim { get; init; }

    [Id(8)]
    public string Cfg { get; init; }

    [Id(9)]
    public float? BoardTemperature { get; init; }

    [Id(10)]
    public TelemetryValidity BoardTemperatureValidity { get; init; }

    [Id(11)]
    public float? OutsideTemperature { get; init; }

    [Id(12)]
    public TelemetryValidity OutsideTemperatureValidity { get; init; }

    [Id(13)]
    public float? BarometerPressure { get; init; }

    [Id(14)]
    public TelemetryValidity BarometerPressureValidity { get; init; }

    [Id(15)]
    public float? Roll { get; init; }

    [Id(16)]
    public TelemetryValidity RollValidity { get; init; }

    [Id(17)]
    public float? Pitch { get; init; }

    [Id(18)]
    public TelemetryValidity PitchValidity { get; init; }

    [Id(19)]
    public bool FanState { get; init; }

    [Id(20)]
    public float? SolarVoltage { get; init; }

    [Id(21)]
    public TelemetryValidity SolarVoltageValidity { get; init; }

    [Id(22)]
    public float? SolarCurrent { get; init; }

    [Id(23)]
    public TelemetryValidity SolarCurrentValidity { get; init; }

    [Id(24)]
    public float? BatteryVoltage { get; init; }

    [Id(25)]
    public TelemetryValidity BatteryVoltageValidity { get; init; }

    [Id(26)]
    public float? LoadVoltage { get; init; }

    [Id(27)]
    public TelemetryValidity LoadVoltageValidity { get; init; }

    [Id(28)]
    public float? LoadCurrent { get; init; }

    [Id(29)]
    public TelemetryValidity LoadCurrentValidity { get; init; }

    [Id(30)]
    public short? BatteryStateOfCharge { get; init; }

    [Id(31)]
    public long UpTime { get; init; }

    [Id(32)]
    public long? Lbcs { get; init; }

    [Id(33)]
    public short? Wmrssi { get; init; }

    [Id(34)]
    public TelemetryValidity WmrssiValidity { get; init; }

    [Id(35)]
    public short? Wmber { get; init; }

    [Id(36)]
    public TelemetryValidity WmberValidity { get; init; }

    [Id(37)]
    public short? NetworkType { get; init; }

    [Id(38)]
    public short? NetworkState { get; init; }

    [Id(39)]
    public long? InventoryVersion { get; init; }

    [Id(40)]
    public long? Fsfree { get; init; }

    [Id(41)]
    public ImmutableList<LockerShutdownReasonType> ShutdownReasons { get; init; }
        = new List<LockerShutdownReasonType>().ToImmutableList();
}

public enum TelemetryValidity
{
    /// <summary>
    /// Value received as expected
    /// </summary>
    OK = 0,
        
    /// <summary>
    /// Null in echo request
    /// </summary>
    NA = 1,
        
    /// <summary>
    /// Error reading data (0xFFFF)
    /// </summary>
    ERROR = 2,
        
    /// <summary>
    /// Date are not yet initialized (0xFFFE)
    /// </summary>
    UNINT = 3
}

public enum LockerShutdownReasonType
{
    OK = 0, // OK
    SDC_NOCARD_PRESENT = 1, // chybí sd karta ověřte správné vložení karty
    SDC_MOUNT_FAILED = 2, // karta nebyla detekována
    SDC_FSSTAT_FAILED = 3, // chybná karta nebo vadny FS
    BLE_INIT_FAILED = 4, // chyba inicializace BLE
    BLE_ADV_FAILED = 5, // chyba ve vysílání BLE
    CFG_LOAD_FAILED = 6, // Nepovedlo se načíst konfiguraci
    CFG_GET_LID_FAILED = 7, // Nepovedlo se načíst ID lockeru z konfigurace
    CAB_STATUS_ERROR = 8, // Nastala chyba u některého sloupce
    INV_LOAD_FAILED = 9, // Nepovedlo se načíst inventář se zásilkami
    WM_SIM_ID_ERROR = 10, // Chyba identifikace sim
    CFG_LOAD_ERROR = 11, // konfigurace nelze přečíst   - NEPOUZIVA SE!!!!
    FW_UPGRADE_RUNNING = 12, // probihajici upgrade fw NEPOUZIVA SE!!!!
    CFG_SYNC_ERROR = 13, // nesedi cfg server / box

    FRAM_ERROR = 14, // chyba cteni/zapisu FRAM
    SDC_ERROR = 15, // chyba zapisu na SD
    SDC_DEV_ERROR = 16, // chyba SDC device
    BL_UPDATE_FAILED = 17,
    CFG_INVALID_PKEY = 18,
    CFG_INVALID_DKEY = 19,
    // Tyto neblokuji produkcni provoz
    CFG_INVALID_SKEY = 20,
    LBCS_ERROR = 21,

    CFG_RECONF = 22,      // chyba synchronizace ISS
    VOLTAGE_WARNING = 23,      // chyba synchronizace ISL  
    // end
    VOLTAGE_ERROR = 24,         //  nizke napeti baterie
    // Tyto neblokuji produkcni provoz
    RTC_TIME_ERROR = 25, // (BLOCK) neni syncnuty cas v RTC
    KV_STORE_ERROR = 26, // (BLOCK) chyba inicializace KV store
    INV_SLOT_DOOR_WARNING = 27, // (NOBLOCK) dvirka jsou v jinem stavu nez maji byt dle INV
    SYS_REBOOT_REMOTE = 28, // (NOBLOCK) ouze docasny stav : box jde do restartu
    SYS_HW_ERROR = 29, // obecna chyba HW, blokujici produkci
    SYS_REBOOT_PENDING = 30, // box jde do restartu.
    SYS_BOOT_START = 31, // Inicializace boxu

    UNKNOWN = 9999
}