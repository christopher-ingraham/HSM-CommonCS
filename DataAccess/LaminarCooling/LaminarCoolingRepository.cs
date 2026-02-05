using DataAccess.LaminarCooling.Models;
using HSM_CommonCS.Database;
using Dapper;

namespace DataAccess.LaminarCooling;

public class LaminarCoolingRepository
{
    private readonly IDbSessionFactory _db;

    public LaminarCoolingRepository(IDbSessionFactory db)
    {
        _db = db;
    }

    public AccStatus? Get(
        int bankNo,
        int bankPos,
        int deviceType)
    {
        using var conn = _db.Open();

        var acc = conn.QuerySingleOrDefault<AccStatus>(
            SqlLoader.Load("LaminarCooling/AccStatus_ByBank.sql"),
            new
            {
                bankNo,
                bankPos,
                deviceType
            });

        if (acc == null)
            return null;

        acc.BankNo = bankNo;
        acc.BankPos = bankPos;
        acc.DeviceType = deviceType;

        return acc;
    }
}
