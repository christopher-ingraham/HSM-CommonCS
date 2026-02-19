namespace CoolingModel.Tests.Builders;

/// <summary>
/// Builds strip input data for physics engine tests.
/// These values match the C++ CoolingModel's typical input ranges.
///
/// NOTE: The actual C# types for these will be created during the port.
/// This builder uses a Dictionary/record pattern that can be adapted once
/// the real C# types (e.g., CoolingStripInput, ChemicalComposition, etc.) exist.
///
/// When the types are ported, update the Build() methods to return the real types.
/// </summary>
public static class StripDataBuilder
{
    // -----------------------------------------------------------------------
    // Physical constants (from C++ PhysicalConstants.h)
    // These are the reference values your ported code must match exactly.
    // -----------------------------------------------------------------------

    public const double AbsoluteZero = 273.15;           // K
    public const double RoomTemperature = 298.15;         // K (25 degC)
    public const double StefanBoltzmann = 5.67e-8;        // W/(m^2*K^4)
    public const double Pi = 3.14159265358979323846;

    // Steel reference properties
    public const double SteelDensity = 7800.0;            // kg/m^3
    public const double SteelSpecificHeat = 1000.0;       // J/(kg*K) — simplified
    public const double SteelThermalConductivity = 27.0;  // W/(m*K)
    public const double SteelEmissivity = 0.95;

    // HTC reference values (from CMCoreConstants.h)
    public const double HtcAir = 100.0;                   // W/(m^2*K)
    public const double HtcWater = 80.0;                   // W/(m^2*K)
    public const double HtcRoll = 10.0;                    // W/(m^2*K)
    public const double HtcDescale = 10000.0;              // W/(m^2*K)

    // -----------------------------------------------------------------------
    // Temperature ranges (from CoolingModel_Constants.h)
    // -----------------------------------------------------------------------

    public const double EntryPyroMinTemp = 575.0 + AbsoluteZero;  // K
    public const double EntryPyroMaxTemp = 1200.0 + AbsoluteZero;  // K
    public const double IntPyroMinTemp = 375.0 + AbsoluteZero;
    public const double IntPyroMaxTemp = 1100.0 + AbsoluteZero;
    public const double ExitPyroMinTemp_High = 375.0 + AbsoluteZero;
    public const double ExitPyroMaxTemp_High = 1100.0 + AbsoluteZero;
    public const double ExitPyroMinTemp_Low = 70.0 + AbsoluteZero;
    public const double ExitPyroMaxTemp_Low = 425.0 + AbsoluteZero;

    public const double WaterTempMin = 3.0 + AbsoluteZero;    // K
    public const double WaterTempMax = 40.0 + AbsoluteZero;   // K
    public const double WaterTempDefault = 21.0 + AbsoluteZero; // K

    // -----------------------------------------------------------------------
    // Typical strip test cases
    // -----------------------------------------------------------------------

    /// <summary>
    /// Standard carbon steel strip — the most common test case.
    /// </summary>
    public static StripTestData StandardCarbonSteel() => new()
    {
        Name = "Standard Carbon Steel",
        ThicknessMm = 10.0,
        WidthMm = 1500.0,
        LengthM = 100.0,
        EntryTemperatureK = 1300.0,       // 1027 degC
        ExitTargetTemperatureK = 873.15,  // 600 degC
        WaterTemperatureK = WaterTempDefault,
        RoomTemperatureK = RoomTemperature,
        StripSpeedMs = 5.0,
        Chemistry = ChemistryPresets.LowCarbonSteel()
    };

    /// <summary>
    /// Thin gauge high-strength steel — tests edge case of fast cooling rates.
    /// </summary>
    public static StripTestData ThinGaugeHSLA() => new()
    {
        Name = "Thin HSLA Steel",
        ThicknessMm = 2.5,
        WidthMm = 2000.0,
        LengthM = 150.0,
        EntryTemperatureK = 1173.15,      // 900 degC
        ExitTargetTemperatureK = 773.15,  // 500 degC
        WaterTemperatureK = WaterTempDefault,
        RoomTemperatureK = RoomTemperature,
        StripSpeedMs = 8.0,
        Chemistry = ChemistryPresets.MicroAlloyedSteel()
    };

    /// <summary>
    /// Heavy gauge plate — tests slow cooling with thick material.
    /// </summary>
    public static StripTestData HeavyGaugePlate() => new()
    {
        Name = "Heavy Gauge Plate",
        ThicknessMm = 25.0,
        WidthMm = 1200.0,
        LengthM = 50.0,
        EntryTemperatureK = 1373.15,       // 1100 degC
        ExitTargetTemperatureK = 923.15,   // 650 degC
        WaterTemperatureK = WaterTempDefault,
        RoomTemperatureK = RoomTemperature,
        StripSpeedMs = 2.0,
        Chemistry = ChemistryPresets.LowCarbonSteel()
    };

    /// <summary>
    /// Extreme cold water case — boundary test.
    /// </summary>
    public static StripTestData ColdWaterExtreme() => new()
    {
        Name = "Cold Water Extreme",
        ThicknessMm = 10.0,
        WidthMm = 1500.0,
        LengthM = 100.0,
        EntryTemperatureK = 1300.0,
        ExitTargetTemperatureK = 873.15,
        WaterTemperatureK = WaterTempMin,  // 3 degC
        RoomTemperatureK = RoomTemperature,
        StripSpeedMs = 5.0,
        Chemistry = ChemistryPresets.LowCarbonSteel()
    };

    /// <summary>
    /// Hot water extreme — boundary test.
    /// </summary>
    public static StripTestData HotWaterExtreme() => new()
    {
        Name = "Hot Water Extreme",
        ThicknessMm = 10.0,
        WidthMm = 1500.0,
        LengthM = 100.0,
        EntryTemperatureK = 1300.0,
        ExitTargetTemperatureK = 873.15,
        WaterTemperatureK = WaterTempMax,  // 40 degC
        RoomTemperatureK = RoomTemperature,
        StripSpeedMs = 5.0,
        Chemistry = ChemistryPresets.LowCarbonSteel()
    };
}

/// <summary>
/// Test data container for a strip cooling scenario.
/// </summary>
public class StripTestData
{
    public string Name { get; init; } = string.Empty;

    // Geometry
    public double ThicknessMm { get; init; }
    public double WidthMm { get; init; }
    public double LengthM { get; init; }

    // Thermal
    public double EntryTemperatureK { get; init; }
    public double ExitTargetTemperatureK { get; init; }
    public double WaterTemperatureK { get; init; }
    public double RoomTemperatureK { get; init; }

    // Speed
    public double StripSpeedMs { get; init; }

    // Chemistry
    public ChemicalCompositionData Chemistry { get; init; } = new();

    // Convenience conversions
    public double ThicknessM => ThicknessMm / 1000.0;
    public double WidthM => WidthMm / 1000.0;
    public double EntryTemperatureC => EntryTemperatureK - StripDataBuilder.AbsoluteZero;
    public double ExitTargetTemperatureC => ExitTargetTemperatureK - StripDataBuilder.AbsoluteZero;

    public override string ToString() => $"{Name} ({ThicknessMm}mm x {WidthMm}mm, {EntryTemperatureC:F0}->{ExitTargetTemperatureC:F0} degC)";
}

/// <summary>
/// Chemical composition data (mirrors C++ C_CHEM_COMP_T with 53 elements).
/// Only the key elements for cooling model are populated; rest default to 0.
/// </summary>
public class ChemicalCompositionData
{
    public double C { get; init; }    // Carbon %
    public double Si { get; init; }   // Silicon %
    public double Mn { get; init; }   // Manganese %
    public double P { get; init; }    // Phosphorus %
    public double S { get; init; }    // Sulfur %
    public double Al { get; init; }   // Aluminum %
    public double Cr { get; init; }   // Chromium %
    public double Ni { get; init; }   // Nickel %
    public double Mo { get; init; }   // Molybdenum %
    public double Cu { get; init; }   // Copper %
    public double Ti { get; init; }   // Titanium %
    public double Nb { get; init; }   // Niobium %
    public double V { get; init; }    // Vanadium %
    public double B { get; init; }    // Boron %
    public double N { get; init; }    // Nitrogen %
    public double Fe { get; init; }   // Iron % (balance)
}

/// <summary>
/// Preset chemical compositions for common steel grades.
/// </summary>
public static class ChemistryPresets
{
    /// <summary>Low carbon steel (typical hot band).</summary>
    public static ChemicalCompositionData LowCarbonSteel() => new()
    {
        C = 0.06, Si = 0.02, Mn = 0.30, P = 0.010, S = 0.008,
        Al = 0.035, Ti = 0.001, N = 0.004, Fe = 99.562
    };

    /// <summary>Medium carbon steel.</summary>
    public static ChemicalCompositionData MediumCarbonSteel() => new()
    {
        C = 0.20, Si = 0.30, Mn = 0.80, P = 0.015, S = 0.010,
        Al = 0.05, Cr = 0.05, Ni = 0.03, Fe = 98.545
    };

    /// <summary>HSLA micro-alloyed steel (Nb-Ti grade).</summary>
    public static ChemicalCompositionData MicroAlloyedSteel() => new()
    {
        C = 0.07, Si = 0.20, Mn = 1.50, P = 0.012, S = 0.003,
        Al = 0.04, Nb = 0.05, Ti = 0.02, V = 0.005, N = 0.005, Fe = 98.095
    };

    /// <summary>High-strength low-alloy (weathering steel).</summary>
    public static ChemicalCompositionData WeatheringSteel() => new()
    {
        C = 0.12, Si = 0.40, Mn = 0.80, P = 0.080, S = 0.020,
        Al = 0.02, Cr = 0.50, Ni = 0.40, Cu = 0.30, Fe = 97.34
    };

    /// <summary>API pipeline steel.</summary>
    public static ChemicalCompositionData ApiPipelineSteel() => new()
    {
        C = 0.05, Si = 0.25, Mn = 1.60, P = 0.010, S = 0.002,
        Al = 0.035, Nb = 0.065, Ti = 0.015, Mo = 0.20, V = 0.005,
        N = 0.004, Fe = 97.764
    };
}
