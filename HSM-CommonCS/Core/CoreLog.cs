namespace HSM_CommonCS.Core
{
    public class CoreLog
    {
        public ILog Log { get; }

        public CoreLog(string appName, string logPath)
        {
            Log = SerilogAdapter.Create(appName, logPath);
        }

    }
}
