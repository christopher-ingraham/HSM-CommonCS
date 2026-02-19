using CoolingModel.Models;
using CoolingModel.Tests.Builders;

namespace CoolingModel.Tests.Domain;

/// <summary>
/// Tests for the CoolingDomain model classes.
/// Validates the object hierarchy, constants, and builder output
/// match the C++ CoolingModel_Types.h layout expectations.
/// </summary>
public class CoolingDomainTests
{
    // -----------------------------------------------------------------------
    // Enum value tests — these MUST match the C++ integer values exactly
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(CoolingZoneType.IntensiveCooling, 1)]
    [InlineData(CoolingZoneType.StandardCooling, 2)]
    [InlineData(CoolingZoneType.TrimmingCooling, 3)]
    public void CoolingZoneType_Values_MatchCpp(CoolingZoneType type, int expected)
    {
        Assert.Equal(expected, (int)type);
    }

    [Theory]
    [InlineData(DeviceType.AccDevice, 1)]
    [InlineData(DeviceType.AccSidesweep, 2)]
    [InlineData(DeviceType.AccBanks, 3)]
    public void DeviceType_Values_MatchCpp(DeviceType type, int expected)
    {
        Assert.Equal(expected, (int)type);
    }

    [Theory]
    [InlineData(BankVerticalPosition.Top, 10)]
    [InlineData(BankVerticalPosition.Bottom, -10)]
    public void BankVerticalPosition_Values_MatchCpp(BankVerticalPosition pos, int expected)
    {
        Assert.Equal(expected, (int)pos);
    }

    // -----------------------------------------------------------------------
    // Constants — these mirror NSGAL plant configuration
    // -----------------------------------------------------------------------

    [Fact]
    public void CoolingProcessStatus_Constants_MatchNsgalConfig()
    {
        Assert.Equal(68, CoolingProcessStatus.TotalBanks);
        Assert.Equal(24, CoolingProcessStatus.TotalIntensiveTopDevices);
        Assert.Equal(48, CoolingProcessStatus.TotalIntensiveBottomDevices);
        Assert.Equal(8, CoolingProcessStatus.TotalSidesweeps);
    }

    [Fact]
    public void IntensiveCoolingBank_MaxDevices_MatchCpp()
    {
        Assert.Equal(4, IntensiveCoolingBank.MaxTopDevices);
        Assert.Equal(4, IntensiveCoolingBank.MaxBottomDevices);
    }

    [Fact]
    public void IntensiveCoolingUnit_MaxBanks_MatchCpp()
    {
        Assert.Equal(8, IntensiveCoolingUnit.MaxTopBanks);
        Assert.Equal(8, IntensiveCoolingUnit.MaxBottomBanks);
    }

    [Fact]
    public void StandardCoolingUnit_MaxBanks_MatchCpp()
    {
        Assert.Equal(6, StandardCoolingUnit.MaxTopBanks);
        Assert.Equal(6, StandardCoolingUnit.MaxBottomBanks);
    }

    [Fact]
    public void TrimmingCoolingUnit_MaxBanks_MatchCpp()
    {
        Assert.Equal(4, TrimmingCoolingUnit.MaxTopBanks);
        Assert.Equal(4, TrimmingCoolingUnit.MaxBottomBanks);
    }

    // -----------------------------------------------------------------------
    // CoolingProcessStatus initialization
    // -----------------------------------------------------------------------

    [Fact]
    public void CoolingProcessStatus_Constructor_InitializesAllArrays()
    {
        var status = new CoolingProcessStatus();

        Assert.Equal(CoolingProcessStatus.TotalBanks, status.TopBankStatus.Length);
        Assert.Equal(CoolingProcessStatus.TotalBanks, status.BottomBankStatus.Length);
        Assert.Equal(CoolingProcessStatus.TotalIntensiveTopDevices, status.TopCoolingDeviceStatus.Length);
        Assert.Equal(CoolingProcessStatus.TotalIntensiveBottomDevices, status.BottomCoolingDeviceStatus.Length);
        Assert.Equal(CoolingProcessStatus.TotalSidesweeps, status.SideSweepStatus.Length);

        // All elements should be non-null
        Assert.All(status.TopBankStatus, s => Assert.NotNull(s));
        Assert.All(status.BottomBankStatus, s => Assert.NotNull(s));
        Assert.All(status.TopCoolingDeviceStatus, s => Assert.NotNull(s));
        Assert.All(status.BottomCoolingDeviceStatus, s => Assert.NotNull(s));
        Assert.All(status.SideSweepStatus, s => Assert.NotNull(s));
    }

    [Fact]
    public void CoolingProcessStatus_Constructor_AllDisabledByDefault()
    {
        var status = new CoolingProcessStatus();

        Assert.All(status.TopBankStatus, s =>
        {
            Assert.False(s.IsEnabledL1);
            Assert.False(s.IsEnabledL2);
        });
    }

    // -----------------------------------------------------------------------
    // Inheritance hierarchy tests
    // -----------------------------------------------------------------------

    [Fact]
    public void IntensiveCoolingZone_IsA_CoolingZone()
    {
        var zone = new IntensiveCoolingZone();
        Assert.IsAssignableFrom<CoolingZone>(zone);
    }

    [Fact]
    public void StandardCoolingZone_IsA_CoolingZone()
    {
        var zone = new StandardCoolingZone();
        Assert.IsAssignableFrom<CoolingZone>(zone);
    }

    [Fact]
    public void TrimmingCoolingZone_IsA_CoolingZone()
    {
        var zone = new TrimmingCoolingZone();
        Assert.IsAssignableFrom<CoolingZone>(zone);
    }

    [Fact]
    public void IntensiveCoolingBank_IsA_CoolingBank()
    {
        var bank = new IntensiveCoolingBank();
        Assert.IsAssignableFrom<CoolingBank>(bank);
    }

    // -----------------------------------------------------------------------
    // Builder tests — validate the test builders produce correct structures
    // -----------------------------------------------------------------------

    [Fact]
    public void NsgalDefault_Creates3Zones()
    {
        var process = CoolingProcessBuilder.NsgalDefault();

        Assert.Equal(3, process.ZoneNum);
        Assert.NotNull(process.IntensiveZone);
        Assert.NotNull(process.StandardZone);
        Assert.NotNull(process.TrimmingZone);
    }

    [Fact]
    public void NsgalDefault_IntensiveZone_Has3Units()
    {
        var process = CoolingProcessBuilder.NsgalDefault();

        Assert.Equal(3, process.IntensiveZone!.NumUnits);
        Assert.Equal(3, process.IntensiveZone.Units.Count);
        Assert.Equal(CoolingZoneType.IntensiveCooling, process.IntensiveZone.ZoneType);
    }

    [Fact]
    public void NsgalDefault_IntensiveZone_EachUnit_Has4TopAnd4BottomBanks()
    {
        var process = CoolingProcessBuilder.NsgalDefault();

        foreach (var unit in process.IntensiveZone!.Units)
        {
            Assert.Equal(4, unit.TopBanks.Count);
            Assert.Equal(4, unit.BottomBanks.Count);
        }
    }

    [Fact]
    public void NsgalDefault_IntensiveZone_TopBanks_Have2Devices()
    {
        var process = CoolingProcessBuilder.NsgalDefault();

        foreach (var unit in process.IntensiveZone!.Units)
        {
            foreach (var bank in unit.TopBanks)
            {
                Assert.Equal(2, bank.TopDevices.Count);
            }
        }
    }

    [Fact]
    public void NsgalDefault_IntensiveZone_BottomBanks_Have4Devices()
    {
        var process = CoolingProcessBuilder.NsgalDefault();

        foreach (var unit in process.IntensiveZone!.Units)
        {
            foreach (var bank in unit.BottomBanks)
            {
                Assert.Equal(4, bank.BottomDevices.Count);
            }
        }
    }

    [Fact]
    public void NsgalDefault_StandardZone_Has6Units()
    {
        var process = CoolingProcessBuilder.NsgalDefault();

        Assert.Equal(6, process.StandardZone!.NumUnits);
        Assert.Equal(6, process.StandardZone.Units.Count);
        Assert.Equal(CoolingZoneType.StandardCooling, process.StandardZone.ZoneType);
    }

    [Fact]
    public void NsgalDefault_TrimmingZone_Has2Units()
    {
        var process = CoolingProcessBuilder.NsgalDefault();

        Assert.Equal(2, process.TrimmingZone!.NumUnits);
        Assert.Equal(2, process.TrimmingZone.Units.Count);
        Assert.Equal(CoolingZoneType.TrimmingCooling, process.TrimmingZone.ZoneType);
    }

    [Fact]
    public void Minimal_CreatesOnlyStandardZone()
    {
        var process = CoolingProcessBuilder.Minimal();

        Assert.Equal(1, process.ZoneNum);
        Assert.Null(process.IntensiveZone);
        Assert.NotNull(process.StandardZone);
        Assert.Null(process.TrimmingZone);
        Assert.Single(process.StandardZone!.Units);
        Assert.Equal(2, process.StandardZone.Units[0].TopBanks.Count);
    }

    // -----------------------------------------------------------------------
    // EquipmentStatusBuilder tests
    // -----------------------------------------------------------------------

    [Fact]
    public void EquipmentStatusBuilder_AllEnabled_AllBanksTrue()
    {
        var status = new EquipmentStatusBuilder().AllEnabled().Build();

        Assert.All(status.TopBankStatus, s =>
        {
            Assert.True(s.IsEnabledL1);
            Assert.True(s.IsEnabledL2);
        });

        Assert.All(status.BottomBankStatus, s =>
        {
            Assert.True(s.IsEnabledL1);
            Assert.True(s.IsEnabledL2);
        });
    }

    [Fact]
    public void EquipmentStatusBuilder_DisableTopBank_OnlyThatBankDisabled()
    {
        var status = new EquipmentStatusBuilder()
            .AllEnabled()
            .DisableTopBank(5)
            .Build();

        // Bank 5 should be disabled
        Assert.False(status.TopBankStatus[4].IsEnabledL1);
        Assert.False(status.TopBankStatus[4].IsEnabledL2);

        // Bank 4 and 6 should still be enabled
        Assert.True(status.TopBankStatus[3].IsEnabledL1);
        Assert.True(status.TopBankStatus[5].IsEnabledL1);
    }

    [Fact]
    public void EquipmentStatusBuilder_L1OnlyEnabled_L2StaysFalse()
    {
        var status = new EquipmentStatusBuilder()
            .AllEnabled()
            .TopBankL1OnlyEnabled(10)
            .Build();

        Assert.True(status.TopBankStatus[9].IsEnabledL1);
        Assert.False(status.TopBankStatus[9].IsEnabledL2);
    }

    [Fact]
    public void EquipmentStatusBuilder_OnlyIntensiveEnabled_StandardAndTrimmingDisabled()
    {
        var status = new EquipmentStatusBuilder()
            .OnlyIntensiveEnabled(12)
            .Build();

        // Banks 1-12 enabled
        for (int i = 0; i < 12; i++)
        {
            Assert.True(status.TopBankStatus[i].IsEnabledL1, $"Top bank {i + 1} should be enabled");
            Assert.True(status.BottomBankStatus[i].IsEnabledL1, $"Bottom bank {i + 1} should be enabled");
        }

        // Banks 13-68 disabled
        for (int i = 12; i < CoolingProcessStatus.TotalBanks; i++)
        {
            Assert.False(status.TopBankStatus[i].IsEnabledL1, $"Top bank {i + 1} should be disabled");
            Assert.False(status.BottomBankStatus[i].IsEnabledL1, $"Bottom bank {i + 1} should be disabled");
        }
    }
}
