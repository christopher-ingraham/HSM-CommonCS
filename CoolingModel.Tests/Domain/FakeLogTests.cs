using CoolingModel.Tests.Fakes;
using HSM_CommonCS.Core;

namespace CoolingModel.Tests.Domain;

/// <summary>
/// Tests for the FakeLog test double itself.
/// Validates the test infrastructure works correctly before using it in other tests.
/// </summary>
public class FakeLogTests
{
    [Fact]
    public void FakeLog_CapturesAllLevels()
    {
        var log = new FakeLog();

        log.Trace("t");
        log.Debug("d");
        log.Info("i");
        log.Warn("w");
        log.Error("e");
        log.Fatal("f");

        Assert.Equal(6, log.Entries.Count);
    }

    [Fact]
    public void FakeLog_HasErrors_TrueOnlyForErrorAndFatal()
    {
        var log = new FakeLog();
        Assert.False(log.HasErrors);

        log.Info("nothing wrong");
        Assert.False(log.HasErrors);

        log.Error("something broke");
        Assert.True(log.HasErrors);
        Assert.Single(log.Errors);
    }

    [Fact]
    public void FakeLog_ContainsMessage_CaseInsensitive()
    {
        var log = new FakeLog();
        log.Info("Cooling zone {ZoneNo} initialized", 3);

        Assert.True(log.ContainsMessage("cooling zone"));
        Assert.True(log.ContainsMessage("COOLING ZONE"));
        Assert.False(log.ContainsMessage("something else"));
    }

    [Fact]
    public void FakeLog_Clear_RemovesAllEntries()
    {
        var log = new FakeLog();
        log.Info("first");
        log.Error("second");

        Assert.Equal(2, log.Entries.Count);

        log.Clear();

        Assert.Empty(log.Entries);
        Assert.False(log.HasErrors);
    }

    [Fact]
    public void FakeLog_SetLevel_PersistsCurrentLevel()
    {
        var log = new FakeLog();
        Assert.Equal(LogLevel.Trace, log.CurrentLevel);

        log.SetLevel(LogLevel.Warning);
        Assert.Equal(LogLevel.Warning, log.CurrentLevel);
    }
}
