using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using HSM_CommonCS.Core;

namespace HSM_CommonCS.Database
{
    internal class OraclePool : IDbSessionFactory
    {
        private readonly string _connectionString;

        public OraclePool(DatabaseConfig cfg)
        {
            _connectionString = cfg.ConnectionString;
        }

        public DbConnection Open()
        {
            var conn = new OracleConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public int? GetProcessLogLevel(string processName)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                        SELECT LOG_LEVEL
                        FROM CFG_PROCESS
                        WHERE RTRIM(PROCESS_ID) = :name";

            var p = cmd.CreateParameter();
            p.ParameterName = "name";
            p.Value = processName;
            cmd.Parameters.Add(p);

            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(result);
        }

        public void Dispose() { }
    }
}
