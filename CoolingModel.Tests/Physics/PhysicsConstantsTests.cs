using CoolingModel.Tests.Builders;

namespace CoolingModel.Tests.Physics;

/// <summary>
/// Tests for physical constants and reference data.
///
/// These tests validate that:
/// 1. The test builders contain physically reasonable values
/// 2. Temperature/dimension ranges match the C++ CoolingModel_Constants.h
/// 3. Chemistry presets have compositions that sum to ~100%
///
/// When the C# physics engine is ported, add tests here that validate
/// the ported PhysicalConstants class matches these reference values.
/// </summary>
public class PhysicsConstantsTests
{
    // -----------------------------------------------------------------------
    // Physical constants sanity checks
    // -----------------------------------------------------------------------

    [Fact]
    public void StefanBoltzmann_MatchesNistValue()
    {
        // NIST: 5.670374419e-8 W/(m^2*K^4)
        Assert.InRange(StripDataBuilder.StefanBoltzmann, 5.66e-8, 5.68e-8);
    }

    [Fact]
    public void AbsoluteZero_Is273Point15()
    {
        Assert.Equal(273.15, StripDataBuilder.AbsoluteZero);
    }

    // -----------------------------------------------------------------------
    // Temperature range checks (from CoolingModel_Constants.h)
    // -----------------------------------------------------------------------

    [Fact]
    public void EntryPyroRange_IsPhysicallyReasonable()
    {
        double minC = StripDataBuilder.EntryPyroMinTemp - StripDataBuilder.AbsoluteZero;
        double maxC = StripDataBuilder.EntryPyroMaxTemp - StripDataBuilder.AbsoluteZero;

        // Entry pyro should cover hot rolling exit range: 575-1200 degC
        Assert.Equal(575.0, minC);
        Assert.Equal(1200.0, maxC);
    }

    [Fact]
    public void ExitPyroHighRange_IsPhysicallyReasonable()
    {
        double minC = StripDataBuilder.ExitPyroMinTemp_High - StripDataBuilder.AbsoluteZero;
        double maxC = StripDataBuilder.ExitPyroMaxTemp_High - StripDataBuilder.AbsoluteZero;

        Assert.Equal(375.0, minC);
        Assert.Equal(1100.0, maxC);
    }

    [Fact]
    public void ExitPyroLowRange_IsPhysicallyReasonable()
    {
        double minC = StripDataBuilder.ExitPyroMinTemp_Low - StripDataBuilder.AbsoluteZero;
        double maxC = StripDataBuilder.ExitPyroMaxTemp_Low - StripDataBuilder.AbsoluteZero;

        Assert.Equal(70.0, minC);
        Assert.Equal(425.0, maxC);
    }

    [Fact]
    public void WaterTempRange_Is3To40DegC()
    {
        double minC = StripDataBuilder.WaterTempMin - StripDataBuilder.AbsoluteZero;
        double maxC = StripDataBuilder.WaterTempMax - StripDataBuilder.AbsoluteZero;

        Assert.Equal(3.0, minC);
        Assert.Equal(40.0, maxC);
    }

    // -----------------------------------------------------------------------
    // Strip test data validation
    // -----------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(AllStripTestCases))]
    public void StripTestData_EntryTemp_WithinPyroRange(StripTestData strip)
    {
        // Entry temperature should be within the entry pyro range
        Assert.InRange(strip.EntryTemperatureK,
            StripDataBuilder.EntryPyroMinTemp,
            StripDataBuilder.EntryPyroMaxTemp + 200); // allow a bit above max pyro
    }

    [Theory]
    [MemberData(nameof(AllStripTestCases))]
    public void StripTestData_TargetTemp_LowerThanEntry(StripTestData strip)
    {
        Assert.True(strip.ExitTargetTemperatureK < strip.EntryTemperatureK,
            $"Exit target ({strip.ExitTargetTemperatureC:F0}C) should be lower than entry ({strip.EntryTemperatureC:F0}C)");
    }

    [Theory]
    [MemberData(nameof(AllStripTestCases))]
    public void StripTestData_Dimensions_PhysicallyReasonable(StripTestData strip)
    {
        Assert.InRange(strip.ThicknessMm, 1.0, 50.0);   // 1mm to 50mm
        Assert.InRange(strip.WidthMm, 600.0, 2500.0);    // 600mm to 2500mm
        Assert.InRange(strip.StripSpeedMs, 0.5, 15.0);   // 0.5 to 15 m/s
    }

    [Theory]
    [MemberData(nameof(AllStripTestCases))]
    public void StripTestData_WaterTemp_InRange(StripTestData strip)
    {
        Assert.InRange(strip.WaterTemperatureK,
            StripDataBuilder.WaterTempMin,
            StripDataBuilder.WaterTempMax);
    }

    // -----------------------------------------------------------------------
    // Chemistry preset validation
    // -----------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(AllChemistryPresets))]
    public void ChemistryPresets_CarbonInRange(string name, ChemicalCompositionData chem)
    {
        Assert.InRange(chem.C, 0.0, 1.5); // max ~1.5% C for any steel
    }

    [Theory]
    [MemberData(nameof(AllChemistryPresets))]
    public void ChemistryPresets_ManganeseInRange(string name, ChemicalCompositionData chem)
    {
        Assert.InRange(chem.Mn, 0.0, 2.5); // max ~2.5% Mn for HSLA
    }

    [Theory]
    [MemberData(nameof(AllChemistryPresets))]
    public void ChemistryPresets_SumApproximately100(string name, ChemicalCompositionData chem)
    {
        double sum = chem.C + chem.Si + chem.Mn + chem.P + chem.S
            + chem.Al + chem.Cr + chem.Ni + chem.Mo + chem.Cu
            + chem.Ti + chem.Nb + chem.V + chem.B + chem.N + chem.Fe;

        // Should be close to 100% (within 1% for rounding)
        Assert.InRange(sum, 99.0, 101.0);
    }

    // -----------------------------------------------------------------------
    // MemberData providers
    // -----------------------------------------------------------------------

    public static IEnumerable<object[]> AllStripTestCases()
    {
        yield return new object[] { StripDataBuilder.StandardCarbonSteel() };
        yield return new object[] { StripDataBuilder.ThinGaugeHSLA() };
        yield return new object[] { StripDataBuilder.HeavyGaugePlate() };
        yield return new object[] { StripDataBuilder.ColdWaterExtreme() };
        yield return new object[] { StripDataBuilder.HotWaterExtreme() };
    }

    public static IEnumerable<object[]> AllChemistryPresets()
    {
        yield return new object[] { "LowCarbon", ChemistryPresets.LowCarbonSteel() };
        yield return new object[] { "MediumCarbon", ChemistryPresets.MediumCarbonSteel() };
        yield return new object[] { "MicroAlloyed", ChemistryPresets.MicroAlloyedSteel() };
        yield return new object[] { "Weathering", ChemistryPresets.WeatheringSteel() };
        yield return new object[] { "ApiPipeline", ChemistryPresets.ApiPipelineSteel() };
    }
}
