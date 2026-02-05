namespace HSM_CommonCS.Core
{
    internal class ProcessLogLevelResolver
    {
        public static LogLevel Resolve(int? dbLevel)
        {
            if (dbLevel == null || dbLevel < 0)
                return LogLevel.Fatal;

            return dbLevel.Value switch
            {
                <= 0 => LogLevel.Fatal,
                <= 1 => LogLevel.Error,
                <= 2 => LogLevel.Warning,
                <= 4 => LogLevel.Information,
                <= 10 => LogLevel.Debug,
                <= 20 => LogLevel.All,
                _ => LogLevel.Information
            };
        }
    }
}
