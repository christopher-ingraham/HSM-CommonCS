using HSM_CommonCS.Database;

namespace HSM_CommonCS.Core
{
    /// <summary>
    /// Application-specific host wrapper. Create one instance of this in each
    /// application's Program.cs to provide convenient typed access to CoreHost.
    /// </summary>
    public sealed class AppHost : IDisposable
    {
        private readonly CoreHost _coreHost;

        /// <summary>Gets the application logger</summary>
        public ILog Log => CoreHost.Log;

        /// <summary>Gets the database session factory</summary>
        public IDbSessionFactory Db => CoreHost.Db;

        private AppHost(CoreHost coreHost)
        {
            _coreHost = coreHost;
        }

        /// <summary>
        /// Starts the application with the given service name.
        /// </summary>
        public static AppHost Start(string serviceName)
        {
            var coreHost = CoreHost.Start(serviceName);
            return new AppHost(coreHost);
        }

        public void Dispose()
        {
            _coreHost?.Dispose();
        }
    }
}
