using DataAccess.LaminarCooling;
using Common.Enums;
using static HSM_CommonCS.Core.CoreHost;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace CoolingModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Start("COOLINGMODEL");  // auto-load config from JSON

            Log.Info("CoolingModel service started");
            Log.Info("Debug Message");

            using var conn = Db.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SYSDATE FROM dual";
            var result = cmd.ExecuteScalar();
            Log.Debug("Database returned: {0}", result);

            Bus.DeclareQueueAsync("COOLINGMODEL");

            var accRepo = new LaminarCoolingRepository(Db);

            var accStatus = accRepo.Get(
                1,
                (int)BankPosition.Bottom,
                3);

            if (accStatus == null)
            {
                Log.Warn("No ACC status found for bank {0}/{1}/{2}",
                1,
                (int)BankPosition.Bottom,
                3);
                return;
            }

            Instance.Dispose();

        }
    }
}
