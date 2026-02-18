using System.Data.Common;
using HSM_CommonCS.Core;
using HSM_CommonCS.Database;
using CoolingModel.Models;

namespace CoolingModel.Data;

/// <summary>
/// Database table structure for TDB_COOLING_ZONE_DATA
/// </summary>
public class TdbCoolingZoneData
{
    public string AreaId { get; set; } = string.Empty;
    public string CenterId { get; set; } = string.Empty;
    public int ZoneNo { get; set; }
    public string ZoneId { get; set; } = string.Empty;
    public int ZoneSeq { get; set; }
    public int ZoneType { get; set; }
    public int NumUnits { get; set; }
    public float Length { get; set; }
    public float Width { get; set; }
    public float MainPressure { get; set; }
    public float WaterTemperature { get; set; }
}

/// <summary>
/// Database table structure for RTDB_ACC_STATUS
/// </summary>
public class RtdbAccStatus
{
    public string AreaId { get; set; } = string.Empty;
    public string CenterId { get; set; } = string.Empty;
    public int BankNo { get; set; }
    public BankVerticalPosition BankPos { get; set; }
    public DeviceType DeviceType { get; set; }
    public int ZoneNo { get; set; }

    public string ZoneId { get; set; } = string.Empty;
    public int BankSeq { get; set; }
    public bool isEnabledL1 { get; set; }
    public bool isEnabledL2 { get; set; }
    public long RtdbAccStatusNo { get; set; }
}

/// <summary>
/// Data access for cooling zone configuration
/// </summary>
public class CoolingZoneRepository : AppService
{
    private const string HSM_AREA_ID = "HSM";
    private const string DC_CENTER_ID = "DC";

    /// <summary>
    /// Load cooling zone configuration from database
    /// </summary>
    public TdbCoolingZoneData? LoadCoolingZoneData(int zoneNo)
    {
        try
        {
            using var conn = Db.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                    SELECT 
                        AREA_ID,
                        CENTER_ID,
                        ZONE_NO,
                        ZONE_ID,
                        ZONE_SEQ,
                        ZONE_TYPE,
                        NUM_UNITS,
                        LENGTH,
                        WIDTH,
                        MAIN_PRES,
                        WATER_TEMP
                    FROM TDB_COOLING_ZONE_DATA
                    WHERE RTRIM(AREA_ID) = :areaId
                      AND RTRIM(CENTER_ID) = :centerId
                      AND ZONE_NO = :zoneNo";

            AddParameter(cmd, "areaId", HSM_AREA_ID);
            AddParameter(cmd, "centerId", DC_CENTER_ID);
            AddParameter(cmd, "zoneNo", zoneNo);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                Log.Trace(
                    "No cooling zone data found in database for area: {AreaId}, center: {CenterId}, zone: {ZoneNo}",
                    HSM_AREA_ID, DC_CENTER_ID, zoneNo);
                return null;
            }

            return new TdbCoolingZoneData
            {
                AreaId = reader.GetString(0).Trim(),
                CenterId = reader.GetString(1).Trim(),
                ZoneNo = reader.GetInt32(2),
                ZoneId = reader.GetString(3).Trim(),
                ZoneSeq = reader.GetInt32(4),
                ZoneType = reader.GetInt32(5),
                NumUnits = reader.GetInt32(6),
                Length = reader.GetFloat(7),
                Width = reader.GetFloat(8),
                MainPressure = reader.GetFloat(9),
                WaterTemperature = reader.GetFloat(10)
            };
        }
        catch (Exception ex)
        {
            Log.Error("Failed to load cooling zone data for zone {ZoneNo}", zoneNo);
            throw;
        }
    }

    /// <summary>
    /// Load equipment status from RTDB_ACC_STATUS table
    /// </summary>
    public RtdbAccStatus? LoadAccStatus(int bankNo, BankVerticalPosition position, DeviceType deviceType)
    {
        try
        {
            using var conn = Db.Open();
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                    SELECT 
                        ZONE_NO,
                        ZONE_ID,
                        BANK_SEQ,
                        BANK_OUT_OF_ORDER,
                        DEVICE_OUT_OF_ORDER,
                        RTDB_ACC_STATUS_NO
                    FROM RTDB_ACC_STATUS
                    WHERE BANK_NO = :bankNo
                      AND BANK_POS = :bankPos
                      AND DEVICE_TYPE = :deviceType";

            AddParameter(cmd, "bankNo", bankNo);
            AddParameter(cmd, "bankPos", (int)position);
            AddParameter(cmd, "deviceType", (int)deviceType);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                Log.Trace(
                    "No ACC status found for bank {BankNo}, position {Position}, device type {DeviceType}",
                    bankNo, position, deviceType);
                return null;
            }

            return new RtdbAccStatus
            {
                AreaId = HSM_AREA_ID,
                CenterId = DC_CENTER_ID,
                BankNo = bankNo,
                BankPos = position,
                DeviceType = deviceType,
                ZoneNo = reader.GetInt32(0),
                ZoneId = reader.GetString(1).Trim(),
                BankSeq = reader.GetInt32(2),
                isEnabledL1 = reader.GetInt32(3) != 0,
                isEnabledL2 = reader.GetInt32(4) != 0,
                RtdbAccStatusNo = reader.GetInt64(5)
            };
        }
        catch (Exception ex)
        {
            Log.Error("Failed to load ACC status for bank {BankNo}, position {Position}, type {DeviceType}",
                bankNo, position, deviceType);
            throw;
        }
    }

    private static void AddParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        cmd.Parameters.Add(param);
    }
}
