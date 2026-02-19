using CoolingModel.Tests.Builders;

namespace CoolingModel.Tests.Physics;

/// <summary>
/// Test harness for the StripThermalModel physics engine.
///
/// =========================================================================
/// IMPORTANT: These tests are SCAFFOLDED but will NOT compile until the
/// C# physics engine is ported. Each test documents the expected behavior
/// from the C++ implementation and should be uncommented as classes are ported.
///
/// Porting order:
///   1. PhysicalConstants.cs    (constants from PhysicalConstants.h)
///   2. IDynamicModel.cs        (interface from DynamicModel.h)
///   3. OdeSolver.cs            (RK4/Euler from Solver.h/.cpp)
///   4. XYTable.cs              (interpolation from XYTable.h)
///   5. WaterHtc.cs             (HTC models from WaterHTC.h/.cpp)
///   6. StripThermalModel.cs    (thermal model from StripThermalModel.h/.cpp)
///   7. CoolingSimulator.cs     (orchestrator from CoolingSimulator.h)
/// =========================================================================
/// </summary>
public class ThermalModelTestHarness
{
    // -----------------------------------------------------------------------
    // ODE Solver tests (Solver.h/.cpp)
    // Uncomment when OdeSolver is ported.
    // -----------------------------------------------------------------------

    /*
    [Fact]
    public void Solver_Euler_SimpleExponentialDecay()
    {
        // dy/dt = -y, y(0) = 1.0
        // Exact solution: y(t) = e^(-t)
        // At t=1: y = 0.3679
        var model = new ExponentialDecayModel();
        var solver = new OdeSolver(SolverMethod.Euler);

        double[] state = { 1.0 };
        solver.Step(model, state, dt: 0.001, steps: 1000);

        Assert.InRange(state[0], 0.36, 0.38); // Euler is only 1st order accurate
    }

    [Fact]
    public void Solver_RK4_SimpleExponentialDecay()
    {
        var model = new ExponentialDecayModel();
        var solver = new OdeSolver(SolverMethod.RK4);

        double[] state = { 1.0 };
        solver.Step(model, state, dt: 0.01, steps: 100);

        // RK4 should be much more accurate than Euler
        Assert.InRange(state[0], 0.3678, 0.3681);
    }

    [Fact]
    public void Solver_RK4_AdaptiveTimeStep_HalvesOnLargeError()
    {
        var model = new StiffModel(); // large derivatives
        var solver = new OdeSolver(SolverMethod.RK4, adaptive: true, tolerance: 1e-6);

        double[] state = { 1.0 };
        var result = solver.Step(model, state, dt: 1.0, steps: 1);

        Assert.True(result.ActualDt < 1.0, "Adaptive solver should reduce dt for stiff systems");
    }
    */

    // -----------------------------------------------------------------------
    // XYTable interpolation tests (XYTable.h)
    // Uncomment when XYTable is ported.
    // -----------------------------------------------------------------------

    /*
    [Fact]
    public void XYTable1D_LinearInterpolation_MidPoint()
    {
        var table = new XYTable1D(
            new double[] { 0, 1, 2, 3 },
            new double[] { 0, 10, 30, 60 });

        Assert.Equal(5.0, table.Interpolate(0.5));
        Assert.Equal(20.0, table.Interpolate(1.5));
    }

    [Fact]
    public void XYTable1D_ExtrapolatesAtBoundaries()
    {
        var table = new XYTable1D(
            new double[] { 1, 2, 3 },
            new double[] { 10, 20, 30 });

        // Below range: clamp or extrapolate (match C++ behavior)
        var result = table.Interpolate(0.0);
        Assert.Equal(10.0, result); // C++ clamps at boundaries
    }

    [Fact]
    public void XYTable2D_BilinearInterpolation()
    {
        // 2D table for OMK HTC model lookup
        var table = new XYTable2D(
            xValues: new double[] { 300, 500, 700, 900 },  // temperature
            yValues: new double[] { 1.0, 5.0, 10.0 },       // flow rate
            zValues: new double[,]
            {
                { 100, 500, 1000 },
                { 200, 800, 1500 },
                { 300, 1200, 2500 },
                { 400, 1500, 3000 }
            });

        double htc = table.Interpolate(600, 7.5);
        Assert.InRange(htc, 800, 1600); // rough check for reasonable HTC
    }
    */

    // -----------------------------------------------------------------------
    // Water HTC model tests (WaterHTC.h/.cpp)
    // Uncomment when WaterHtc is ported.
    // -----------------------------------------------------------------------

    /*
    [Theory]
    [MemberData(nameof(AllStripCases))]
    public void WaterHtc_Nucor_ReturnsPositiveHtc(StripTestData strip)
    {
        var pars = HtcNucorModelPars.Default();
        var htc = WaterHtc.ComputeNucor(
            surfaceTemperatureK: strip.EntryTemperatureK,
            waterTemperatureK: strip.WaterTemperatureK,
            flowRateLpm: 50.0,
            pars: pars);

        Assert.True(htc > 0, "HTC must be positive");
        Assert.InRange(htc, 100, 50000); // physically reasonable range W/(m^2*K)
    }

    [Theory]
    [MemberData(nameof(AllStripCases))]
    public void WaterHtc_Guo_ReturnsPositiveHtc(StripTestData strip)
    {
        var pars = HtcGuoModelPars.Default();
        var htc = WaterHtc.ComputeGuo(
            surfaceTemperatureK: strip.EntryTemperatureK,
            waterTemperatureK: strip.WaterTemperatureK,
            flowRateLpm: 50.0,
            pars: pars);

        Assert.True(htc > 0, "HTC must be positive");
    }

    [Fact]
    public void WaterHtc_HigherFlow_IncreasesHtc()
    {
        var pars = HtcNucorModelPars.Default();

        var htcLow = WaterHtc.ComputeNucor(1000 + 273.15, 21 + 273.15, flowRateLpm: 10, pars);
        var htcHigh = WaterHtc.ComputeNucor(1000 + 273.15, 21 + 273.15, flowRateLpm: 100, pars);

        Assert.True(htcHigh > htcLow, "Higher water flow should increase HTC");
    }

    [Fact]
    public void WaterHtc_HigherSurfaceTemp_ChangesHtc()
    {
        // The relationship is non-linear (film boiling, transition, nucleate boiling)
        // Just verify it changes and stays positive
        var pars = HtcNucorModelPars.Default();

        var htc600 = WaterHtc.ComputeNucor(600 + 273.15, 21 + 273.15, 50, pars);
        var htc900 = WaterHtc.ComputeNucor(900 + 273.15, 21 + 273.15, 50, pars);

        Assert.True(htc600 > 0);
        Assert.True(htc900 > 0);
        Assert.NotEqual(htc600, htc900);
    }
    */

    // -----------------------------------------------------------------------
    // StripThermalModel tests (StripThermalModel.h/.cpp)
    // Uncomment when StripThermalModel is ported.
    // -----------------------------------------------------------------------

    /*
    [Theory]
    [MemberData(nameof(AllStripCases))]
    public void ThermalModel_AirCooling_TemperatureDecreasesMonotonically(StripTestData strip)
    {
        // Air-only cooling (no water) — temperature should decrease monotonically
        var model = new StripThermalModel(
            thicknessM: strip.ThicknessM,
            nodes: 21,
            ambientTemperatureK: strip.RoomTemperatureK,
            emissivity: StripDataBuilder.SteelEmissivity);

        model.SetUniformTemperature(strip.EntryTemperatureK);
        var solver = new OdeSolver(SolverMethod.RK4);

        double prevAvgTemp = model.AverageTemperature;
        for (int i = 0; i < 100; i++)
        {
            solver.Step(model, dt: 0.1);
            double avgTemp = model.AverageTemperature;
            Assert.True(avgTemp <= prevAvgTemp + 0.01,
                $"Temperature should not increase during air cooling (step {i})");
            prevAvgTemp = avgTemp;
        }
    }

    [Theory]
    [MemberData(nameof(AllStripCases))]
    public void ThermalModel_WaterCooling_CoolsFasterThanAir(StripTestData strip)
    {
        // Same strip, air vs water — water should cool faster
        var airModel = CreateAirCooledModel(strip);
        var waterModel = CreateWaterCooledModel(strip);
        var solver = new OdeSolver(SolverMethod.RK4);

        for (int i = 0; i < 100; i++)
        {
            solver.Step(airModel, dt: 0.1);
            solver.Step(waterModel, dt: 0.1);
        }

        Assert.True(waterModel.AverageTemperature < airModel.AverageTemperature,
            "Water cooling should produce lower temperature than air cooling");
    }

    [Fact]
    public void ThermalModel_SymmetricCooling_ProfileIsSymmetric()
    {
        // If top and bottom HTC are equal, temperature profile should be symmetric
        var model = new StripThermalModel(
            thicknessM: 0.010, nodes: 21,
            ambientTemperatureK: 300, emissivity: 0.95);

        model.SetUniformTemperature(1200);
        model.SetTopHtc(5000);
        model.SetBottomHtc(5000);

        var solver = new OdeSolver(SolverMethod.RK4);
        for (int i = 0; i < 50; i++)
            solver.Step(model, dt: 0.1);

        var profile = model.TemperatureProfile;
        int mid = profile.Length / 2;

        // Check symmetry: T[i] should equal T[n-1-i]
        for (int i = 0; i < mid; i++)
        {
            Assert.InRange(Math.Abs(profile[i] - profile[profile.Length - 1 - i]),
                0, 0.1); // within 0.1K
        }
    }

    [Fact]
    public void ThermalModel_EnergyConservation_HeatLostEqualsConduction()
    {
        // Total energy change should equal integrated heat flux at boundaries
        var model = new StripThermalModel(
            thicknessM: 0.010, nodes: 21,
            ambientTemperatureK: 300, emissivity: 0.95);

        model.SetUniformTemperature(1000);
        double initialEnergy = model.TotalEnergy;

        var solver = new OdeSolver(SolverMethod.RK4);
        double totalHeatLost = 0;
        for (int i = 0; i < 100; i++)
        {
            totalHeatLost += model.SurfaceHeatFlux * 0.1; // flux * dt
            solver.Step(model, dt: 0.1);
        }

        double finalEnergy = model.TotalEnergy;
        double energyChange = initialEnergy - finalEnergy;

        // Energy conservation: lost energy should match boundary flux
        Assert.InRange(Math.Abs(energyChange - totalHeatLost) / energyChange,
            0, 0.05); // within 5%
    }
    */

    // -----------------------------------------------------------------------
    // MemberData provider
    // -----------------------------------------------------------------------

    public static IEnumerable<object[]> AllStripCases()
    {
        yield return new object[] { StripDataBuilder.StandardCarbonSteel() };
        yield return new object[] { StripDataBuilder.ThinGaugeHSLA() };
        yield return new object[] { StripDataBuilder.HeavyGaugePlate() };
        yield return new object[] { StripDataBuilder.ColdWaterExtreme() };
        yield return new object[] { StripDataBuilder.HotWaterExtreme() };
    }
}
