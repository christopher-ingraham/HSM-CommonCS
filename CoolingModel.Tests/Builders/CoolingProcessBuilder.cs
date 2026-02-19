using CoolingModel.Models;

namespace CoolingModel.Tests.Builders;

/// <summary>
/// Fluent builder for CoolingProcess test fixtures.
/// Creates realistic NSGAL-like cooling configurations.
///
/// Usage:
///   var process = new CoolingProcessBuilder()
///       .WithIntensiveZone(units: 3, banksPerUnit: 4)
///       .WithStandardZone(units: 6, banksPerUnit: 4)
///       .WithTrimmingZone(units: 2, banksPerUnit: 16)
///       .Build();
///
/// Or use the preset:
///   var process = CoolingProcessBuilder.NsgalDefault();
/// </summary>
public class CoolingProcessBuilder
{
    private IntensiveCoolingZone? _intensiveZone;
    private StandardCoolingZone? _standardZone;
    private TrimmingCoolingZone? _trimmingZone;

    // Default pyrometer positions (meters from entry reference)
    private float _entryScanPyroPos = 0f;
    private float _entryPyroPos = 2.5f;
    private float _interPyroPos = 30f;
    private float _exitPyroPos = 95f;
    private float _exitScanPyroPos = 97f;

    public CoolingProcessBuilder WithPyroPositions(
        float entryScan, float entry, float inter, float exit, float exitScan)
    {
        _entryScanPyroPos = entryScan;
        _entryPyroPos = entry;
        _interPyroPos = inter;
        _exitPyroPos = exit;
        _exitScanPyroPos = exitScan;
        return this;
    }

    public CoolingProcessBuilder WithIntensiveZone(
        int units = 3,
        int banksPerUnit = 4,
        int topDevicesPerBank = 2,
        int bottomDevicesPerBank = 4,
        float pressure = 0.95f,
        float waterTemp = 21f)
    {
        var zone = new IntensiveCoolingZone
        {
            ZoneId = "INTENSIVE",
            ZoneNo = 1,
            ZoneSeq = 1,
            ZoneType = CoolingZoneType.IntensiveCooling,
            NumUnits = units,
            Length = units * 4000f,  // ~4m per unit
            Width = 2000f,
            MainPressure = pressure,
            WaterTemperature = waterTemp
        };

        for (int u = 0; u < units; u++)
        {
            var unit = new IntensiveCoolingUnit
            {
                UnitNo = u + 1,
                UnitSeq = u + 1,
                Length = 4000f,
                Width = 2000f
            };

            for (int b = 0; b < banksPerUnit; b++)
            {
                var topBank = BuildIntensiveBank(u * banksPerUnit + b + 1, b + 1,
                    BankVerticalPosition.Top, topDevicesPerBank, pressure, waterTemp);
                var bottomBank = BuildIntensiveBank(u * banksPerUnit + b + 1, b + 1,
                    BankVerticalPosition.Bottom, bottomDevicesPerBank, pressure, waterTemp);

                unit.TopBanks.Add(topBank);
                unit.BottomBanks.Add(bottomBank);
            }

            zone.Units.Add(unit);
        }

        _intensiveZone = zone;
        return this;
    }

    public CoolingProcessBuilder WithStandardZone(
        int units = 6,
        int banksPerUnit = 4,
        float pressure = 0.95f,
        float waterTemp = 21f)
    {
        var zone = new StandardCoolingZone
        {
            ZoneId = "STANDARD",
            ZoneNo = 2,
            ZoneSeq = 2,
            ZoneType = CoolingZoneType.StandardCooling,
            NumUnits = units,
            Length = units * 5000f,
            Width = 2000f,
            MainPressure = pressure,
            WaterTemperature = waterTemp
        };

        for (int u = 0; u < units; u++)
        {
            var unit = new StandardCoolingUnit
            {
                UnitNo = u + 1,
                UnitSeq = u + 1,
                Length = 5000f,
                Width = 2000f
            };

            for (int b = 0; b < banksPerUnit; b++)
            {
                int bankNo = (_intensiveZone?.Units.Sum(x => x.TopBanks.Count) ?? 0) + u * banksPerUnit + b + 1;
                unit.TopBanks.Add(BuildStandardBank(bankNo, b + 1, BankVerticalPosition.Top, pressure, waterTemp));
                unit.BottomBanks.Add(BuildStandardBank(bankNo, b + 1, BankVerticalPosition.Bottom, pressure, waterTemp));
            }

            zone.Units.Add(unit);
        }

        _standardZone = zone;
        return this;
    }

    public CoolingProcessBuilder WithTrimmingZone(
        int units = 2,
        int banksPerUnit = 16,
        float pressure = 0.95f,
        float waterTemp = 21f)
    {
        var zone = new TrimmingCoolingZone
        {
            ZoneId = "TRIMMING",
            ZoneNo = 3,
            ZoneSeq = 3,
            ZoneType = CoolingZoneType.TrimmingCooling,
            NumUnits = units,
            Length = units * 8000f,
            Width = 2000f,
            MainPressure = pressure,
            WaterTemperature = waterTemp
        };

        for (int u = 0; u < units; u++)
        {
            var unit = new TrimmingCoolingUnit
            {
                UnitNo = u + 1,
                UnitSeq = u + 1,
                Length = 8000f,
                Width = 2000f
            };

            int baseBankNo = (_intensiveZone?.Units.Sum(x => x.TopBanks.Count) ?? 0)
                + (_standardZone?.Units.Sum(x => x.TopBanks.Count) ?? 0)
                + u * banksPerUnit;

            for (int b = 0; b < banksPerUnit; b++)
            {
                unit.TopBanks.Add(BuildStandardBank(baseBankNo + b + 1, b + 1, BankVerticalPosition.Top, pressure, waterTemp));
                unit.BottomBanks.Add(BuildStandardBank(baseBankNo + b + 1, b + 1, BankVerticalPosition.Bottom, pressure, waterTemp));
            }

            zone.Units.Add(unit);
        }

        _trimmingZone = zone;
        return this;
    }

    public CoolingProcess Build()
    {
        int zoneCount = (_intensiveZone != null ? 1 : 0)
            + (_standardZone != null ? 1 : 0)
            + (_trimmingZone != null ? 1 : 0);

        return new CoolingProcess
        {
            ZoneNum = zoneCount,
            EntryScanPyroPos = _entryScanPyroPos,
            EntryPyroPos = _entryPyroPos,
            InterPyroPos = _interPyroPos,
            ExitPyroPos = _exitPyroPos,
            ExitScanPyroPos = _exitScanPyroPos,
            IntensiveZone = _intensiveZone,
            StandardZone = _standardZone,
            TrimmingZone = _trimmingZone
        };
    }

    /// <summary>
    /// Build a full NSGAL-like configuration:
    /// 3 zones (intensive, standard, trimming), 68 total banks.
    /// </summary>
    public static CoolingProcess NsgalDefault()
    {
        return new CoolingProcessBuilder()
            .WithIntensiveZone(units: 3, banksPerUnit: 4, topDevicesPerBank: 2, bottomDevicesPerBank: 4)
            .WithStandardZone(units: 6, banksPerUnit: 4)
            .WithTrimmingZone(units: 2, banksPerUnit: 16)
            .Build();
    }

    /// <summary>
    /// Minimal config with just 1 zone, 1 unit, 2 banks — for fast unit tests.
    /// </summary>
    public static CoolingProcess Minimal()
    {
        return new CoolingProcessBuilder()
            .WithStandardZone(units: 1, banksPerUnit: 2)
            .Build();
    }

    // --- Private helpers ---

    private static IntensiveCoolingBank BuildIntensiveBank(
        int bankNo, int bankSeq, BankVerticalPosition position,
        int deviceCount, float pressure, float waterTemp)
    {
        var bank = new IntensiveCoolingBank
        {
            BankNo = bankNo,
            BankSeq = bankSeq,
            Position = position,
            Length = 1500f,   // mm
            Width = 2000f,    // mm
            MainPressure = pressure,
            WaterTemperature = waterTemp
        };

        var devices = position == BankVerticalPosition.Top ? bank.TopDevices : bank.BottomDevices;
        for (int d = 0; d < deviceCount; d++)
        {
            devices.Add(new CoolingDevice
            {
                DeviceNo = d + 1,
                DeviceSeq = d + 1,
                DeviceType = 1,
                DeviceDescription = position == BankVerticalPosition.Top ? "U-Tube" : "Bottom Spray",
                WetLength = 600f,
                Efficiency = 1.0f
            });
        }

        return bank;
    }

    private static CoolingBank BuildStandardBank(
        int bankNo, int bankSeq, BankVerticalPosition position,
        float pressure, float waterTemp)
    {
        return new CoolingBank
        {
            BankNo = bankNo,
            BankSeq = bankSeq,
            Position = position,
            Length = 1500f,
            Width = 2000f,
            MainPressure = pressure,
            WaterTemperature = waterTemp
        };
    }
}
