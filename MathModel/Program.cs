using HSM_CommonCS.Core;


namespace MathModel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CoreHost.Start("MATHMODEL");  // auto-load config from JSON

            Log.InfoMsg("MathModel service started");
            Log.DebugMsg("Debug Message");

            using var conn = CoreHost.Instance.Db.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SYSDATE FROM dual";
            var result = cmd.ExecuteScalar();
            Log.DebugMsg("Database returned: {0}", result);

            CoreHost.Instance.Dispose();
        }
    }
}
