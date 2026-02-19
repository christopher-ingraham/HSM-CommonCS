using CoolingModel.Models;

namespace CoolingModel.Tests.Builders;

/// <summary>
/// Fluent builder for CoolingProcessStatus test fixtures.
///
/// Usage:
///   var status = new EquipmentStatusBuilder()
///       .AllBanksEnabled()
///       .DisableTopBank(5)
///       .DisableBottomBank(12)
///       .Build();
/// </summary>
public class EquipmentStatusBuilder
{
    private readonly CoolingProcessStatus _status = new();

    /// <summary>Enable all banks (top + bottom), all devices, all sidesweeps.</summary>
    public EquipmentStatusBuilder AllEnabled()
    {
        for (int i = 0; i < CoolingProcessStatus.TotalBanks; i++)
        {
            _status.TopBankStatus[i].IsEnabledL1 = true;
            _status.TopBankStatus[i].IsEnabledL2 = true;
            _status.BottomBankStatus[i].IsEnabledL1 = true;
            _status.BottomBankStatus[i].IsEnabledL2 = true;
        }

        for (int i = 0; i < CoolingProcessStatus.TotalIntensiveTopDevices; i++)
        {
            _status.TopCoolingDeviceStatus[i].IsEnabledL1 = true;
            _status.TopCoolingDeviceStatus[i].IsEnabledL2 = true;
        }

        for (int i = 0; i < CoolingProcessStatus.TotalIntensiveBottomDevices; i++)
        {
            _status.BottomCoolingDeviceStatus[i].IsEnabledL1 = true;
            _status.BottomCoolingDeviceStatus[i].IsEnabledL2 = true;
        }

        for (int i = 0; i < CoolingProcessStatus.TotalSidesweeps; i++)
        {
            _status.SideSweepStatus[i].IsEnabledL1 = true;
            _status.SideSweepStatus[i].IsEnabledL2 = true;
        }

        return this;
    }

    /// <summary>All banks disabled (default state).</summary>
    public EquipmentStatusBuilder AllDisabled()
    {
        // CoolingProcessStatus constructor already initializes everything to default (false)
        return this;
    }

    /// <summary>Disable a specific top bank (1-based index).</summary>
    public EquipmentStatusBuilder DisableTopBank(int bankNo)
    {
        _status.TopBankStatus[bankNo - 1].IsEnabledL1 = false;
        _status.TopBankStatus[bankNo - 1].IsEnabledL2 = false;
        return this;
    }

    /// <summary>Disable a specific bottom bank (1-based index).</summary>
    public EquipmentStatusBuilder DisableBottomBank(int bankNo)
    {
        _status.BottomBankStatus[bankNo - 1].IsEnabledL1 = false;
        _status.BottomBankStatus[bankNo - 1].IsEnabledL2 = false;
        return this;
    }

    /// <summary>Set L1 enabled but L2 disabled for a top bank (simulates L1/L2 mismatch).</summary>
    public EquipmentStatusBuilder TopBankL1OnlyEnabled(int bankNo)
    {
        _status.TopBankStatus[bankNo - 1].IsEnabledL1 = true;
        _status.TopBankStatus[bankNo - 1].IsEnabledL2 = false;
        return this;
    }

    /// <summary>Enable only the intensive zone banks (1-12 in NSGAL).</summary>
    public EquipmentStatusBuilder OnlyIntensiveEnabled(int intensiveBankCount = 12)
    {
        AllDisabled();
        for (int i = 0; i < intensiveBankCount && i < CoolingProcessStatus.TotalBanks; i++)
        {
            _status.TopBankStatus[i].IsEnabledL1 = true;
            _status.TopBankStatus[i].IsEnabledL2 = true;
            _status.BottomBankStatus[i].IsEnabledL1 = true;
            _status.BottomBankStatus[i].IsEnabledL2 = true;
        }
        return this;
    }

    /// <summary>Enable only the standard zone banks (13-36 in NSGAL).</summary>
    public EquipmentStatusBuilder OnlyStandardEnabled(int startBank = 13, int endBank = 36)
    {
        AllDisabled();
        for (int i = startBank - 1; i < endBank && i < CoolingProcessStatus.TotalBanks; i++)
        {
            _status.TopBankStatus[i].IsEnabledL1 = true;
            _status.TopBankStatus[i].IsEnabledL2 = true;
            _status.BottomBankStatus[i].IsEnabledL1 = true;
            _status.BottomBankStatus[i].IsEnabledL2 = true;
        }
        return this;
    }

    /// <summary>Enable only the trimming zone banks (37-68 in NSGAL).</summary>
    public EquipmentStatusBuilder OnlyTrimmingEnabled(int startBank = 37, int endBank = 68)
    {
        AllDisabled();
        for (int i = startBank - 1; i < endBank && i < CoolingProcessStatus.TotalBanks; i++)
        {
            _status.TopBankStatus[i].IsEnabledL1 = true;
            _status.TopBankStatus[i].IsEnabledL2 = true;
            _status.BottomBankStatus[i].IsEnabledL1 = true;
            _status.BottomBankStatus[i].IsEnabledL2 = true;
        }
        return this;
    }

    public CoolingProcessStatus Build() => _status;
}
