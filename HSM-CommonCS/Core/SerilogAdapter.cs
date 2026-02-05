using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace HSM_CommonCS.Core
{
    internal class SerilogAdapter : ILog
    {
        private readonly ILogger _logger;
        private readonly LoggingLevelSwitch _levelSwitch;

        public SerilogAdapter(ILogger logger, LoggingLevelSwitch levelSwitch)
        {
            _logger = logger;
            _levelSwitch = levelSwitch;
        }

        public static ILog Create(string appName, string logPath)
        {
            var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);

            var serilog = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.WithProperty("App", appName)
                .WriteTo.Console()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{App}{Timestamp:HH:mm:ss.ss}{Level:u3}]{Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            return new SerilogAdapter(serilog, levelSwitch);
        }

        public void SetLevel(LogLevel level)
        {
            _levelSwitch.MinimumLevel = level switch
            {
                LogLevel.Trace => LogEventLevel.Verbose,
                LogLevel.Debug => LogEventLevel.Debug,
                LogLevel.Information => LogEventLevel.Information,
                LogLevel.Warning => LogEventLevel.Warning,
                LogLevel.Error => LogEventLevel.Error,
                LogLevel.Fatal => LogEventLevel.Fatal,
                _ => LogEventLevel.Information
            };
        }

        public LogLevel CurrentLevel => _levelSwitch.MinimumLevel switch
        {
            LogEventLevel.Verbose => LogLevel.Trace,
            LogEventLevel.Debug => LogLevel.Debug,
            LogEventLevel.Information => LogLevel.Information,
            LogEventLevel.Warning => LogLevel.Warning,
            LogEventLevel.Error => LogLevel.Error,
            LogEventLevel.Fatal => LogLevel.Fatal,
            _ => LogLevel.Information
        };

        public void Trace(string message, params object[] args) => _logger.Verbose(message, args);

        public void Debug(string message, params object[] args) => _logger.Debug(message, args);

        public void Info(string message, params object[] args) => _logger.Information(message, args);

        public void Warn(string message, params object[] args) => _logger.Warning(message, args);

        public void Error(string message, params object[] args) => _logger.Error(message, args);

        public void Fatal(string message, params object[] args) => _logger.Fatal(message, args);

    }
}
