using HSM_CommonCS.Core;

namespace CoolingModel.Tests.Fakes;

/// <summary>
/// In-memory logger for testing. Captures all log messages so tests can assert on them.
/// </summary>
public class FakeLog : ILog
{
    private readonly List<LogEntry> _entries = new();
    private LogLevel _level = LogLevel.Trace;

    public LogLevel CurrentLevel => _level;

    /// <summary>All captured log entries.</summary>
    public IReadOnlyList<LogEntry> Entries => _entries;

    /// <summary>All messages at Error or Fatal level.</summary>
    public IReadOnlyList<LogEntry> Errors =>
        _entries.Where(e => e.Level >= LogLevel.Error).ToList();

    /// <summary>True if any Error or Fatal was logged.</summary>
    public bool HasErrors => _entries.Any(e => e.Level >= LogLevel.Error);

    public void Trace(string message, params object[] args) => Log(LogLevel.Trace, message, args);
    public void Debug(string message, params object[] args) => Log(LogLevel.Debug, message, args);
    public void Info(string message, params object[] args) => Log(LogLevel.Information, message, args);
    public void Warn(string message, params object[] args) => Log(LogLevel.Warning, message, args);
    public void Error(string message, params object[] args) => Log(LogLevel.Error, message, args);
    public void Fatal(string message, params object[] args) => Log(LogLevel.Fatal, message, args);

    public void SetLevel(LogLevel level) => _level = level;

    public void Clear() => _entries.Clear();

    private void Log(LogLevel level, string message, object[] args)
    {
        _entries.Add(new LogEntry(DateTime.UtcNow, level, message, args));
    }

    /// <summary>
    /// Check if any log entry contains the given substring (case-insensitive).
    /// </summary>
    public bool ContainsMessage(string substring) =>
        _entries.Any(e => e.Message.Contains(substring, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Dump all log entries to console (useful for debugging failed tests).
    /// </summary>
    public void DumpToConsole()
    {
        foreach (var entry in _entries)
        {
            Console.WriteLine($"[{entry.Level}] {entry.Message}");
        }
    }
}

public record LogEntry(DateTime Timestamp, LogLevel Level, string Message, object[] Args);
