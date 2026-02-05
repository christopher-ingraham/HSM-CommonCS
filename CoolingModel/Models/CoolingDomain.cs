namespace CoolingModel.Models
{
    /// <summary>
    /// Cooling zone types
    /// </summary>
    public enum CoolingZoneType
    {
        IntensiveCooling = 1,
        StandardCooling = 2,
        TrimmingCooling = 3
    }

    /// <summary>
    /// Device types for cooling equipment
    /// </summary>
    public enum DeviceType
    {
        AccBanks = 3,
        AccDevice = 1,
        AccSidesweep = 2
    }

    /// <summary>
    /// Vertical position of cooling banks
    /// </summary>
    public enum BankVerticalPosition
    {
        Top = 10,
        Bottom = -10
    }

    /// <summary>
    /// Equipment status tracking
    /// </summary>
    public class EquipmentStatus
    {
        public bool IsEnabledL1 { get; set; }
        public bool IsEnabledL2 { get; set; }
    }

    /// <summary>
    /// Cooling device (nozzle, spray header, etc.)
    /// </summary>
    public class CoolingDevice
    {
        public int DeviceNo { get; set; }
        public int DeviceSeq { get; set; }
        public int DeviceType { get; set; }
        public string DeviceDescription { get; set; } = string.Empty;
        public float WetLength { get; set; }  // mm - wet area length
        public float Efficiency { get; set; }  // 1.0 = nominal flow
    }

    /// <summary>
    /// Base cooling bank (row of cooling devices)
    /// </summary>
    public class CoolingBank
    {
        public int BankNo { get; set; }
        public int BankSeq { get; set; }
        public BankVerticalPosition Position { get; set; }
        public float Length { get; set; }  // mm
        public float Width { get; set; }   // mm
        public float MainPressure { get; set; }
        public float WaterTemperature { get; set; }
    }

    /// <summary>
    /// Intensive cooling bank with top and bottom devices
    /// </summary>
    public class IntensiveCoolingBank : CoolingBank
    {
        public const int MaxTopDevices = 4;
        public const int MaxBottomDevices = 4;

        public List<CoolingDevice> TopDevices { get; set; } = new();
        public List<CoolingDevice> BottomDevices { get; set; } = new();
    }

    /// <summary>
    /// Base cooling unit (group of banks)
    /// </summary>
    public class CoolingUnit
    {
        public int UnitNo { get; set; }
        public int UnitSeq { get; set; }
        public float Length { get; set; }  // mm
        public float Width { get; set; }   // mm
    }

    /// <summary>
    /// Intensive cooling unit
    /// </summary>
    public class IntensiveCoolingUnit : CoolingUnit
    {
        public const int MaxTopBanks = 8;
        public const int MaxBottomBanks = 8;

        public List<IntensiveCoolingBank> TopBanks { get; set; } = new();
        public List<IntensiveCoolingBank> BottomBanks { get; set; } = new();
    }

    /// <summary>
    /// Standard cooling unit
    /// </summary>
    public class StandardCoolingUnit : CoolingUnit
    {
        public const int MaxTopBanks = 6;
        public const int MaxBottomBanks = 6;

        public List<CoolingBank> TopBanks { get; set; } = new();
        public List<CoolingBank> BottomBanks { get; set; } = new();
    }

    /// <summary>
    /// Trimming cooling unit
    /// </summary>
    public class TrimmingCoolingUnit : CoolingUnit
    {
        public const int MaxTopBanks = 4;
        public const int MaxBottomBanks = 4;

        public List<CoolingBank> TopBanks { get; set; } = new();
        public List<CoolingBank> BottomBanks { get; set; } = new();
    }

    /// <summary>
    /// Base cooling zone
    /// </summary>
    public class CoolingZone
    {
        public string ZoneId { get; set; } = string.Empty;
        public int ZoneNo { get; set; }
        public int ZoneSeq { get; set; }
        public CoolingZoneType ZoneType { get; set; }
        public int NumUnits { get; set; }
        public float Length { get; set; }  // mm
        public float Width { get; set; }   // mm
        public float MainPressure { get; set; }
        public float WaterTemperature { get; set; }
    }

    /// <summary>
    /// Intensive cooling zone
    /// </summary>
    public class IntensiveCoolingZone : CoolingZone
    {
        public const int MaxUnits = 2;
        public List<IntensiveCoolingUnit> Units { get; set; } = new();
    }

    /// <summary>
    /// Standard cooling zone
    /// </summary>
    public class StandardCoolingZone : CoolingZone
    {
        public const int MaxUnits = 2;
        public List<StandardCoolingUnit> Units { get; set; } = new();
    }

    /// <summary>
    /// Trimming cooling zone
    /// </summary>
    public class TrimmingCoolingZone : CoolingZone
    {
        public const int MaxUnits = 1;
        public List<TrimmingCoolingUnit> Units { get; set; } = new();
    }

    /// <summary>
    /// Complete cooling process configuration
    /// </summary>
    public class CoolingProcess
    {
        public int ZoneNum { get; set; }

        // Pyrometer positions (mm from process beginning)
        public float EntryScanPyroPos { get; set; }
        public float EntryPyroPos { get; set; }
        public float InterPyroPos { get; set; }
        public float ExitPyroPos { get; set; }
        public float ExitScanPyroPos { get; set; }
        public float EntryPyroFirstBankDist { get; set; }
        public float ExitPyroLastBankDist { get; set; }

        public IntensiveCoolingZone? IntensiveZone { get; set; }
        public StandardCoolingZone? StandardZone { get; set; }
        public TrimmingCoolingZone? TrimmingZone { get; set; }
    }

    /// <summary>
    /// Real-time status of all cooling equipment
    /// </summary>
    public class CoolingProcessStatus
    {
        public const int TotalBanks = 68;
        public const int TotalIntensiveTopDevices = 24;
        public const int TotalIntensiveBottomDevices = 48;
        public const int TotalSidesweeps = 8;

        public EquipmentStatus[] TopBankStatus { get; set; } = new EquipmentStatus[TotalBanks];
        public EquipmentStatus[] BottomBankStatus { get; set; } = new EquipmentStatus[TotalBanks];
        public EquipmentStatus[] TopCoolingDeviceStatus { get; set; } = new EquipmentStatus[TotalIntensiveTopDevices];
        public EquipmentStatus[] BottomCoolingDeviceStatus { get; set; } = new EquipmentStatus[TotalIntensiveBottomDevices];
        public EquipmentStatus[] SideSweepStatus { get; set; } = new EquipmentStatus[TotalSidesweeps];

        public CoolingProcessStatus()
        {
            for (int i = 0; i < TotalBanks; i++)
            {
                TopBankStatus[i] = new EquipmentStatus();
                BottomBankStatus[i] = new EquipmentStatus();
            }

            for (int i = 0; i < TotalIntensiveTopDevices; i++)
            {
                TopCoolingDeviceStatus[i] = new EquipmentStatus();
            }

            for (int i = 0; i < TotalIntensiveBottomDevices; i++)
            {
                BottomCoolingDeviceStatus[i] = new EquipmentStatus();
            }

            for (int i = 0; i < TotalSidesweeps; i++)
            {
                SideSweepStatus[i] = new EquipmentStatus();
            }
        }
    }
}
