namespace HSM_CommonCS.Core
{
    public interface ILog
    {
        void Trace(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(string message, params object[] args);
        void Fatal(string message, params object[] args);

        void SetLevel(LogLevel level);
        LogLevel CurrentLevel { get; }
    }
}
