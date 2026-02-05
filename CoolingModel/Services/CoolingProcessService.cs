using HSM_CommonCS.Core;
using CoolingModel.Models;
using CoolingModel.Data;

namespace CoolingModel.Services
{
    /// <summary>
    /// Service for initializing and managing cooling process configuration
    /// </summary>
    public class CoolingProcessService : AppService
    {
        private readonly CoolingZoneRepository _repository;

        public CoolingProcessService()
        {
            _repository = new CoolingZoneRepository();
        }

        /// <summary>
        /// Load complete cooling process configuration from database
        /// </summary>
        public CoolingProcess LoadCoolingProcess(int expectedZoneCount)
        {
            Log.Info("Starting cooling process initialization with {ZoneCount} zones", expectedZoneCount);

            var process = new CoolingProcess
            {
                ZoneNum = expectedZoneCount
            };

            int zonesLoaded = 0;

            for (int zoneNo = 1; zoneNo <= expectedZoneCount; zoneNo++)
            {
                var zoneData = _repository.LoadCoolingZoneData(zoneNo);

                if (zoneData == null)
                {
                    Log.Error(
                        "Failed to load cooling zone data from database for zone {ZoneNo}. Initialization failed",
                        zoneNo);
                    throw new InvalidOperationException($"Failed to load zone {zoneNo}");
                }

                var zoneType = (CoolingZoneType)zoneData.ZoneType;

                switch (zoneType)
                {
                    case CoolingZoneType.IntensiveCooling:
                        process.IntensiveZone = InitializeIntensiveZone(zoneData);
                        Log.Debug("Initialized intensive cooling zone {ZoneNo}", zoneNo);
                        break;

                    case CoolingZoneType.StandardCooling:
                        process.StandardZone = InitializeStandardZone(zoneData);
                        Log.Debug("Initialized standard cooling zone {ZoneNo}", zoneNo);
                        break;

                    case CoolingZoneType.TrimmingCooling:
                        process.TrimmingZone = InitializeTrimmingZone(zoneData);
                        Log.Debug("Initialized trimming cooling zone {ZoneNo}", zoneNo);
                        break;

                    default:
                        Log.Error(
                            "Unknown cooling zone type {ZoneType} loaded from database: {ZoneId}. Cannot initialize",
                            zoneData.ZoneType, zoneData.ZoneId);
                        throw new InvalidOperationException($"Unknown zone type: {zoneData.ZoneType}");
                }

                zonesLoaded++;
            }

            Log.Info("Cooling process initialized successfully with {ZonesLoaded} zones", zonesLoaded);
            return process;
        }

        /// <summary>
        /// Load real-time equipment status from database
        /// </summary>
        public CoolingProcessStatus LoadCoolingProcessStatus()
        {
            Log.Info("Loading cooling process equipment status from database");

            var status = new CoolingProcessStatus();

            // Load top bank status
            for (int i = 0; i < CoolingProcessStatus.TotalBanks; i++)
            {
                var bankStatus = _repository.LoadAccStatus(
                    i + 1, 
                    BankVerticalPosition.Top, 
                    DeviceType.AccBanks);

                if (bankStatus != null)
                {
                    status.TopBankStatus[i].IsEnabledL1 = bankStatus.isEnabledL1;
                    status.TopBankStatus[i].IsEnabledL2 = bankStatus.isEnabledL2;
                }
            }

            // Load bottom bank status
            for (int i = 0; i < CoolingProcessStatus.TotalBanks; i++)
            {
                var bankStatus = _repository.LoadAccStatus(
                    i + 1, 
                    BankVerticalPosition.Bottom, 
                    DeviceType.AccBanks);

                if (bankStatus != null)
                {
                    status.BottomBankStatus[i].IsEnabledL1 = bankStatus.isEnabledL1;
                    status.BottomBankStatus[i].IsEnabledL2 = bankStatus.isEnabledL2;
                }
            }

            // Load top device status
            for (int i = 0; i < CoolingProcessStatus.TotalIntensiveTopDevices; i++)
            {
                var deviceStatus = _repository.LoadAccStatus(
                    i + 1, 
                    BankVerticalPosition.Top, 
                    DeviceType.AccDevice);

                if (deviceStatus != null)
                {
                    status.TopCoolingDeviceStatus[i].IsEnabledL1 = deviceStatus.isEnabledL1;
                    status.TopCoolingDeviceStatus[i].IsEnabledL2 = deviceStatus.isEnabledL2;
                }
            }

            // Load bottom device status
            for (int i = 0; i < CoolingProcessStatus.TotalIntensiveBottomDevices; i++)
            {
                var deviceStatus = _repository.LoadAccStatus(
                    i + 1, 
                    BankVerticalPosition.Bottom, 
                    DeviceType.AccDevice);

                if (deviceStatus != null)
                {
                    status.BottomCoolingDeviceStatus[i].IsEnabledL1 = deviceStatus.isEnabledL1;
                    status.BottomCoolingDeviceStatus[i].IsEnabledL2 = deviceStatus.isEnabledL2;
                }
            }

            // Load sidesweep spray status
            for (int i = 0; i < CoolingProcessStatus.TotalSidesweeps; i++)
            {
                var sidesweepStatus = _repository.LoadAccStatus(
                    i + 1, 
                    BankVerticalPosition.Bottom, 
                    DeviceType.AccSidesweep);

                if (sidesweepStatus != null)
                {
                    status.SideSweepStatus[i].IsEnabledL1 = sidesweepStatus.isEnabledL1;
                    status.SideSweepStatus[i].IsEnabledL2 = sidesweepStatus.isEnabledL2;
                }
            }

            Log.Info("Cooling process equipment status loaded successfully");
            return status;
        }

        private IntensiveCoolingZone InitializeIntensiveZone(TdbCoolingZoneData zoneData)
        {
            return new IntensiveCoolingZone
            {
                ZoneId = zoneData.ZoneId,
                ZoneNo = zoneData.ZoneNo,
                ZoneSeq = zoneData.ZoneSeq,
                ZoneType = CoolingZoneType.IntensiveCooling,
                NumUnits = zoneData.NumUnits,
                Length = zoneData.Length,
                Width = zoneData.Width,
                MainPressure = zoneData.MainPressure,
                WaterTemperature = zoneData.WaterTemperature
            };
        }

        private StandardCoolingZone InitializeStandardZone(TdbCoolingZoneData zoneData)
        {
            return new StandardCoolingZone
            {
                ZoneId = zoneData.ZoneId,
                ZoneNo = zoneData.ZoneNo,
                ZoneSeq = zoneData.ZoneSeq,
                ZoneType = CoolingZoneType.StandardCooling,
                NumUnits = zoneData.NumUnits,
                Length = zoneData.Length,
                Width = zoneData.Width,
                MainPressure = zoneData.MainPressure,
                WaterTemperature = zoneData.WaterTemperature
            };
        }

        private TrimmingCoolingZone InitializeTrimmingZone(TdbCoolingZoneData zoneData)
        {
            return new TrimmingCoolingZone
            {
                ZoneId = zoneData.ZoneId,
                ZoneNo = zoneData.ZoneNo,
                ZoneSeq = zoneData.ZoneSeq,
                ZoneType = CoolingZoneType.TrimmingCooling,
                NumUnits = zoneData.NumUnits,
                Length = zoneData.Length,
                Width = zoneData.Width,
                MainPressure = zoneData.MainPressure,
                WaterTemperature = zoneData.WaterTemperature
            };
        }
    }
}
