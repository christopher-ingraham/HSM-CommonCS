using CoolingModel.Models;
using CoolingModel.Services;
using CoolingModel.Tests.Builders;
using CoolingModel.Tests.Fakes;

namespace CoolingModel.Tests.Services;

/// <summary>
/// Tests for CoolingProcessService.
/// Uses FakeDbSessionFactory to simulate Oracle database responses.
///
/// IMPORTANT: These tests use CoreHost singleton. They must not run in parallel.
/// </summary>
[Collection("CoreHost")]
public class CoolingProcessServiceTests : IDisposable
{
    private readonly FakeLog _log;
    private readonly FakeDbSessionFactory _db;

    public CoolingProcessServiceTests()
    {
        _log = new FakeLog();
        _db = new FakeDbSessionFactory();
        TestCoreHostBootstrap.Initialize(_log, _db);
    }

    public void Dispose()
    {
        TestCoreHostBootstrap.Reset();
    }

    // -----------------------------------------------------------------------
    // LoadCoolingProcess — happy path
    // -----------------------------------------------------------------------

    [Fact]
    public void LoadCoolingProcess_ThreeZones_ReturnsAllZoneTypes()
    {
        // Arrange: queue 3 zone result sets (one per zone query)
        _db.EnqueueResultSet(DbResultBuilder.IntensiveZoneRow(zoneNo: 1, numUnits: 3));
        _db.EnqueueResultSet(DbResultBuilder.StandardZoneRow(zoneNo: 2, numUnits: 6));
        _db.EnqueueResultSet(DbResultBuilder.TrimmingZoneRow(zoneNo: 3, numUnits: 2));

        var service = new CoolingProcessService();

        // Act
        var process = service.LoadCoolingProcess(expectedZoneCount: 3);

        // Assert
        Assert.Equal(3, process.ZoneNum);
        Assert.NotNull(process.IntensiveZone);
        Assert.NotNull(process.StandardZone);
        Assert.NotNull(process.TrimmingZone);
    }

    [Fact]
    public void LoadCoolingProcess_IntensiveZone_MapsFieldsCorrectly()
    {
        _db.EnqueueResultSet(DbResultBuilder.CoolingZoneRow(
            zoneNo: 1, zoneId: "IC_ZONE", zoneSeq: 1, zoneType: 1,
            numUnits: 3, length: 12500f, width: 1900f,
            mainPressure: 1.2f, waterTemperature: 23f));

        var service = new CoolingProcessService();
        var process = service.LoadCoolingProcess(expectedZoneCount: 1);

        var zone = process.IntensiveZone!;
        Assert.Equal("IC_ZONE", zone.ZoneId);
        Assert.Equal(1, zone.ZoneNo);
        Assert.Equal(1, zone.ZoneSeq);
        Assert.Equal(CoolingZoneType.IntensiveCooling, zone.ZoneType);
        Assert.Equal(3, zone.NumUnits);
        Assert.Equal(12500f, zone.Length);
        Assert.Equal(1900f, zone.Width);
        Assert.Equal(1.2f, zone.MainPressure);
        Assert.Equal(23f, zone.WaterTemperature);
    }

    [Fact]
    public void LoadCoolingProcess_StandardZone_MapsFieldsCorrectly()
    {
        _db.EnqueueResultSet(DbResultBuilder.CoolingZoneRow(
            zoneNo: 1, zoneId: "STD_ZONE", zoneSeq: 2, zoneType: 2,
            numUnits: 6, length: 30000f, width: 2000f,
            mainPressure: 0.95f, waterTemperature: 21f));

        var service = new CoolingProcessService();
        var process = service.LoadCoolingProcess(expectedZoneCount: 1);

        var zone = process.StandardZone!;
        Assert.Equal("STD_ZONE", zone.ZoneId);
        Assert.Equal(CoolingZoneType.StandardCooling, zone.ZoneType);
        Assert.Equal(6, zone.NumUnits);
    }

    [Fact]
    public void LoadCoolingProcess_TrimmingZone_MapsFieldsCorrectly()
    {
        _db.EnqueueResultSet(DbResultBuilder.CoolingZoneRow(
            zoneNo: 1, zoneId: "TRIM_ZONE", zoneSeq: 3, zoneType: 3,
            numUnits: 2, length: 16000f, width: 2000f,
            mainPressure: 0.8f, waterTemperature: 20f));

        var service = new CoolingProcessService();
        var process = service.LoadCoolingProcess(expectedZoneCount: 1);

        var zone = process.TrimmingZone!;
        Assert.Equal("TRIM_ZONE", zone.ZoneId);
        Assert.Equal(CoolingZoneType.TrimmingCooling, zone.ZoneType);
        Assert.Equal(2, zone.NumUnits);
    }

    [Fact]
    public void LoadCoolingProcess_LogsStartAndCompletion()
    {
        _db.EnqueueResultSet(DbResultBuilder.IntensiveZoneRow());

        var service = new CoolingProcessService();
        service.LoadCoolingProcess(expectedZoneCount: 1);

        Assert.True(_log.ContainsMessage("Starting cooling process initialization"));
        Assert.True(_log.ContainsMessage("initialized successfully"));
    }

    // -----------------------------------------------------------------------
    // LoadCoolingProcess — error paths
    // -----------------------------------------------------------------------

    [Fact]
    public void LoadCoolingProcess_MissingZone_ThrowsInvalidOperation()
    {
        // Return empty result for zone 1
        _db.EnqueueResultSet(DbResultBuilder.EmptyCoolingZoneResult());

        var service = new CoolingProcessService();

        var ex = Assert.Throws<InvalidOperationException>(
            () => service.LoadCoolingProcess(expectedZoneCount: 1));

        Assert.Contains("Failed to load zone 1", ex.Message);
    }

    [Fact]
    public void LoadCoolingProcess_MissingZone_LogsError()
    {
        _db.EnqueueResultSet(DbResultBuilder.EmptyCoolingZoneResult());

        var service = new CoolingProcessService();

        try { service.LoadCoolingProcess(expectedZoneCount: 1); } catch { }

        Assert.True(_log.HasErrors);
        Assert.True(_log.ContainsMessage("Failed to load cooling zone data"));
    }

    [Fact]
    public void LoadCoolingProcess_UnknownZoneType_ThrowsInvalidOperation()
    {
        // Zone type 99 is invalid
        _db.EnqueueResultSet(DbResultBuilder.CoolingZoneRow(
            zoneNo: 1, zoneId: "BAD_ZONE", zoneSeq: 1, zoneType: 99, numUnits: 1));

        var service = new CoolingProcessService();

        var ex = Assert.Throws<InvalidOperationException>(
            () => service.LoadCoolingProcess(expectedZoneCount: 1));

        Assert.Contains("Unknown zone type: 99", ex.Message);
    }

    // -----------------------------------------------------------------------
    // LoadCoolingProcessStatus — happy path
    // -----------------------------------------------------------------------

    [Fact]
    public void LoadCoolingProcessStatus_EnabledBank_SetsL1AndL2True()
    {
        // Need to enqueue results for ALL queries that LoadCoolingProcessStatus makes:
        // 68 top banks + 68 bottom banks + 24 top devices + 48 bottom devices + 8 sidesweeps = 216
        EnqueueAccStatusResults(
            topBankEnabled: true,
            bottomBankEnabled: true,
            topDeviceEnabled: true,
            bottomDeviceEnabled: true,
            sidesweepEnabled: true);

        var service = new CoolingProcessService();
        var status = service.LoadCoolingProcessStatus();

        Assert.True(status.TopBankStatus[0].IsEnabledL1);
        Assert.True(status.TopBankStatus[0].IsEnabledL2);
        Assert.True(status.BottomBankStatus[0].IsEnabledL1);
    }

    [Fact]
    public void LoadCoolingProcessStatus_DisabledBank_SetsL1AndL2False()
    {
        // DisabledBankStatus sends bankOutOfOrder=0, deviceOutOfOrder=0
        // Repo maps: isEnabledL1 = (0 != 0) = false
        EnqueueAccStatusResults(
            topBankEnabled: false,
            bottomBankEnabled: false,
            topDeviceEnabled: false,
            bottomDeviceEnabled: false,
            sidesweepEnabled: false);

        var service = new CoolingProcessService();
        var status = service.LoadCoolingProcessStatus();

        Assert.False(status.TopBankStatus[0].IsEnabledL1);
        Assert.False(status.TopBankStatus[0].IsEnabledL2);
    }

    [Fact]
    public void LoadCoolingProcessStatus_MissingStatus_DefaultsToDisabled()
    {
        // Enqueue empty results for everything
        int totalQueries = CoolingProcessStatus.TotalBanks * 2
            + CoolingProcessStatus.TotalIntensiveTopDevices
            + CoolingProcessStatus.TotalIntensiveBottomDevices
            + CoolingProcessStatus.TotalSidesweeps;

        for (int i = 0; i < totalQueries; i++)
            _db.EnqueueResultSet(DbResultBuilder.EmptyAccStatusResult());

        var service = new CoolingProcessService();
        var status = service.LoadCoolingProcessStatus();

        // When no data found, status defaults to false (not updated)
        Assert.False(status.TopBankStatus[0].IsEnabledL1);
        Assert.False(status.TopBankStatus[0].IsEnabledL2);
    }

    [Fact]
    public void LoadCoolingProcessStatus_LogsStartAndCompletion()
    {
        int totalQueries = CoolingProcessStatus.TotalBanks * 2
            + CoolingProcessStatus.TotalIntensiveTopDevices
            + CoolingProcessStatus.TotalIntensiveBottomDevices
            + CoolingProcessStatus.TotalSidesweeps;

        for (int i = 0; i < totalQueries; i++)
            _db.EnqueueResultSet(DbResultBuilder.EmptyAccStatusResult());

        var service = new CoolingProcessService();
        service.LoadCoolingProcessStatus();

        Assert.True(_log.ContainsMessage("Loading cooling process equipment status"));
        Assert.True(_log.ContainsMessage("equipment status loaded successfully"));
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private void EnqueueAccStatusResults(
        bool topBankEnabled, bool bottomBankEnabled,
        bool topDeviceEnabled, bool bottomDeviceEnabled,
        bool sidesweepEnabled)
    {
        int outOfOrder = 1; // "out of order" value

        // Top banks (68)
        for (int i = 0; i < CoolingProcessStatus.TotalBanks; i++)
        {
            _db.EnqueueResultSet(topBankEnabled
                ? DbResultBuilder.EnabledBankStatus(bankSeq: i + 1)
                : DbResultBuilder.DisabledBankStatus(bankSeq: i + 1));
        }

        // Bottom banks (68)
        for (int i = 0; i < CoolingProcessStatus.TotalBanks; i++)
        {
            _db.EnqueueResultSet(bottomBankEnabled
                ? DbResultBuilder.EnabledBankStatus(bankSeq: i + 1)
                : DbResultBuilder.DisabledBankStatus(bankSeq: i + 1));
        }

        // Top devices (24)
        for (int i = 0; i < CoolingProcessStatus.TotalIntensiveTopDevices; i++)
        {
            _db.EnqueueResultSet(topDeviceEnabled
                ? DbResultBuilder.EnabledBankStatus(bankSeq: i + 1)
                : DbResultBuilder.DisabledBankStatus(bankSeq: i + 1));
        }

        // Bottom devices (48)
        for (int i = 0; i < CoolingProcessStatus.TotalIntensiveBottomDevices; i++)
        {
            _db.EnqueueResultSet(bottomDeviceEnabled
                ? DbResultBuilder.EnabledBankStatus(bankSeq: i + 1)
                : DbResultBuilder.DisabledBankStatus(bankSeq: i + 1));
        }

        // Sidesweeps (8)
        for (int i = 0; i < CoolingProcessStatus.TotalSidesweeps; i++)
        {
            _db.EnqueueResultSet(sidesweepEnabled
                ? DbResultBuilder.EnabledBankStatus(bankSeq: i + 1)
                : DbResultBuilder.DisabledBankStatus(bankSeq: i + 1));
        }
    }
}
