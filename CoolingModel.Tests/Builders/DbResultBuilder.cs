using System.Data;

namespace CoolingModel.Tests.Builders;

/// <summary>
/// Builds DataTables that mimic Oracle query results.
/// Used with FakeDbSessionFactory to simulate database responses.
///
/// Each builder method matches a specific SQL query in the repository layer
/// and returns the correct column structure.
/// </summary>
public static class DbResultBuilder
{
    // -----------------------------------------------------------------------
    // TDB_COOLING_ZONE_DATA results (matches CoolingZoneRepository.LoadCoolingZoneData)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Build a TDB_COOLING_ZONE_DATA result row.
    /// Columns: AREA_ID, CENTER_ID, ZONE_NO, ZONE_ID, ZONE_SEQ, ZONE_TYPE, NUM_UNITS, LENGTH, WIDTH, MAIN_PRES, WATER_TEMP
    /// </summary>
    public static DataTable CoolingZoneRow(
        int zoneNo,
        string zoneId,
        int zoneSeq,
        int zoneType,
        int numUnits,
        float length = 12000f,
        float width = 2000f,
        float mainPressure = 0.95f,
        float waterTemperature = 21f,
        string areaId = "HSM",
        string centerId = "DC")
    {
        var dt = CreateCoolingZoneTable();
        dt.Rows.Add(areaId, centerId, zoneNo, zoneId, zoneSeq, zoneType, numUnits,
            length, width, mainPressure, waterTemperature);
        return dt;
    }

    /// <summary>Build an intensive zone row (zoneType=1).</summary>
    public static DataTable IntensiveZoneRow(int zoneNo = 1, int numUnits = 3) =>
        CoolingZoneRow(zoneNo, "INTENSIVE", 1, 1, numUnits, 12000f, 2000f, 0.95f, 21f);

    /// <summary>Build a standard zone row (zoneType=2).</summary>
    public static DataTable StandardZoneRow(int zoneNo = 2, int numUnits = 6) =>
        CoolingZoneRow(zoneNo, "STANDARD", 2, 2, numUnits, 30000f, 2000f, 0.95f, 21f);

    /// <summary>Build a trimming zone row (zoneType=3).</summary>
    public static DataTable TrimmingZoneRow(int zoneNo = 3, int numUnits = 2) =>
        CoolingZoneRow(zoneNo, "TRIMMING", 3, 3, numUnits, 16000f, 2000f, 0.95f, 21f);

    /// <summary>Empty cooling zone result (zone not found).</summary>
    public static DataTable EmptyCoolingZoneResult() => CreateCoolingZoneTable();

    private static DataTable CreateCoolingZoneTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("AREA_ID", typeof(string));
        dt.Columns.Add("CENTER_ID", typeof(string));
        dt.Columns.Add("ZONE_NO", typeof(int));
        dt.Columns.Add("ZONE_ID", typeof(string));
        dt.Columns.Add("ZONE_SEQ", typeof(int));
        dt.Columns.Add("ZONE_TYPE", typeof(int));
        dt.Columns.Add("NUM_UNITS", typeof(int));
        dt.Columns.Add("LENGTH", typeof(float));
        dt.Columns.Add("WIDTH", typeof(float));
        dt.Columns.Add("MAIN_PRES", typeof(float));
        dt.Columns.Add("WATER_TEMP", typeof(float));
        return dt;
    }

    // -----------------------------------------------------------------------
    // RTDB_ACC_STATUS results (matches CoolingZoneRepository.LoadAccStatus)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Build an RTDB_ACC_STATUS result row.
    /// Columns: ZONE_NO, ZONE_ID, BANK_SEQ, BANK_OUT_OF_ORDER, DEVICE_OUT_OF_ORDER, RTDB_ACC_STATUS_NO
    ///
    /// NOTE: CoolingZoneRepository maps these as:
    ///   isEnabledL1 = reader.GetInt32(3) != 0  (BANK_OUT_OF_ORDER)
    ///   isEnabledL2 = reader.GetInt32(4) != 0  (DEVICE_OUT_OF_ORDER)
    /// So nonzero = isEnabled=true. The column naming is misleading.
    /// </summary>
    public static DataTable AccStatusRow(
        int zoneNo = 1,
        string zoneId = "INTENSIVE",
        int bankSeq = 1,
        int bankOutOfOrder = 0,
        int deviceOutOfOrder = 0,
        long statusNo = 1)
    {
        var dt = CreateAccStatusTable();
        dt.Rows.Add(zoneNo, zoneId, bankSeq, bankOutOfOrder, deviceOutOfOrder, statusNo);
        return dt;
    }

    /// <summary>Build an enabled bank status row (nonzero values -> isEnabled=true).</summary>
    public static DataTable EnabledBankStatus(int zoneNo = 1, string zoneId = "INTENSIVE", int bankSeq = 1) =>
        AccStatusRow(zoneNo, zoneId, bankSeq, bankOutOfOrder: 1, deviceOutOfOrder: 1);

    /// <summary>Build a disabled bank status row (zero values -> isEnabled=false).</summary>
    public static DataTable DisabledBankStatus(int zoneNo = 1, string zoneId = "INTENSIVE", int bankSeq = 1) =>
        AccStatusRow(zoneNo, zoneId, bankSeq, bankOutOfOrder: 0, deviceOutOfOrder: 0);

    /// <summary>Empty status result (not found).</summary>
    public static DataTable EmptyAccStatusResult() => CreateAccStatusTable();

    private static DataTable CreateAccStatusTable()
    {
        var dt = new DataTable();
        dt.Columns.Add("ZONE_NO", typeof(int));
        dt.Columns.Add("ZONE_ID", typeof(string));
        dt.Columns.Add("BANK_SEQ", typeof(int));
        dt.Columns.Add("BANK_OUT_OF_ORDER", typeof(int));
        dt.Columns.Add("DEVICE_OUT_OF_ORDER", typeof(int));
        dt.Columns.Add("RTDB_ACC_STATUS_NO", typeof(long));
        return dt;
    }

    // -----------------------------------------------------------------------
    // SYSDATE result (matches the database connection test)
    // -----------------------------------------------------------------------

    /// <summary>Build a SYSDATE scalar result.</summary>
    public static DataTable SysdateResult(DateTime? date = null)
    {
        var dt = new DataTable();
        dt.Columns.Add("SYSDATE", typeof(DateTime));
        dt.Rows.Add(date ?? DateTime.Now);
        return dt;
    }
}
