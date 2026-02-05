using Common.Enums;
using CoolingModel.Models;
using CoolingModel.Services;
using DataAccess.LaminarCooling;
using static HSM_CommonCS.Core.CoreHost;


namespace CoolingModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Start("COOLINGMODEL");  // auto-load config from JSON

            try
            {
                Log.Info("CoolingModel service started");


                // Test database connection
                using (var conn = Db.Open())
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT SYSDATE FROM dual";
                    var result = cmd.ExecuteScalar();
                    Log.Debug("Database connection verified: {DbTime}", result);
                }

                // Initialize cooling process service
                var coolingService = new CoolingProcessService();

                // Load cooling process configuration (assuming 3 zones: intensive, standard, trimming)
                var coolingProcess = coolingService.LoadCoolingProcess(expectedZoneCount: 3);

                Log.Info("Cooling process initialized:");
                if (coolingProcess.IntensiveZone != null)
                {
                    Log.Info("  - Intensive Zone {ZoneNo}: {ZoneId}, {NumUnits} units",
                        coolingProcess.IntensiveZone.ZoneNo,
                        coolingProcess.IntensiveZone.ZoneId,
                        coolingProcess.IntensiveZone.NumUnits);
                }
                if (coolingProcess.StandardZone != null)
                {
                    Log.Info("  - Standard Zone {ZoneNo}: {ZoneId}, {NumUnits} units",
                        coolingProcess.StandardZone.ZoneNo,
                        coolingProcess.StandardZone.ZoneId,
                        coolingProcess.StandardZone.NumUnits);
                }
                if (coolingProcess.TrimmingZone != null)
                {
                    Log.Info("  - Trimming Zone {ZoneNo}: {ZoneId}, {NumUnits} units",
                        coolingProcess.TrimmingZone.ZoneNo,
                        coolingProcess.TrimmingZone.ZoneId,
                        coolingProcess.TrimmingZone.NumUnits);
                }

                // Load real-time equipment status
                var equipmentStatus = coolingService.LoadCoolingProcessStatus();



                Log.Info("Equipment status loaded: {EnabledBanks} of {TotalBanks} Top banks enabled",
                    equipmentStatus.TopBankStatus.Count(b => b.IsEnabledL1), CoolingProcessStatus.TotalBanks);
                Log.Info("Equipment status loaded: {EnabledBanks} of {TotalBanks} Bot banks enabled",
                    equipmentStatus.BottomBankStatus.Count(b => b.IsEnabledL1), CoolingProcessStatus.TotalBanks);

                Log.Info("CoolingModel initialization complete");

            }
            catch (Exception ex)
            {
                Log.Fatal("Fatal error during CoolingModel initialization", ex);
                throw;
            }
            Instance.Dispose();

        }
    }
}
