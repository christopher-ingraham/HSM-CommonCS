using HSM_CommonCS.Database;
using HSM_CommonCS.Messaging;
using Microsoft.Extensions.Configuration;

namespace HSM_CommonCS.Core
{
    /// <summary>
    /// Core application host providing logging, database, and lifecycle management.
    /// Thread-safe singleton that auto-disposes on application exit.
    /// </summary>
    public class CoreHost : IDisposable
    {
        private static readonly object _lock = new();
        private static CoreHost? _instance;
        private bool _disposed;

        // Public static accessors - available everywhere
        public static ILog Log => Instance._log;
        public static IDbSessionFactory Db => Instance._db;
        public static IMessageBus Bus => Instance._bus;


        private readonly ILog _log;
        private readonly IDbSessionFactory _db;
        private readonly IMessageBus _bus;
        private readonly string _applicationName;

        private CoreHost(CoreConfig cfg)
        {
            _applicationName = cfg.ApplicationName;

            var coreLog = new CoreLog(cfg.ApplicationName, cfg.LogPath);
            _log = coreLog.Log;
            _log.Info("Logger started with default level INFORMATION");

            _bus = new RabbitMessageBus(_log, cfg.RabbitConfig);
            _bus.StartAsync().GetAwaiter().GetResult(); 
            _bus.DeclareQueueAsync(cfg.ApplicationName);
            _log.Info("RabbitMQ messaging initialized");


            _db = new OraclePool(cfg.Database);

            var logLevelDatabaseValue = ((OraclePool)_db).GetProcessLogLevel(cfg.ApplicationName);
            var newLevel = ProcessLogLevelResolver.Resolve(logLevelDatabaseValue);
            _log.SetLevel(newLevel);

            _log.Info("Log level set to {New} from (CFG_PROCESS)", newLevel);
            _log.Trace($"{cfg.ApplicationName} started");
        }

        /// <summary>
        /// Gets the singleton instance. Throws if not initialized.
        /// </summary>
        public static CoreHost Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        "CoreHost not initialized. Call CoreHost.Start() first.");
                return _instance;
            }
        }

        /// <summary>
        /// Starts the application host with the given service name.
        /// Loads configuration from SharedConfig/appsettings.json.
        /// </summary>
        /// <param name="serviceName">Name of the service (must match appsettings.json)</param>
        /// <returns>The CoreHost instance for use with 'using' statement</returns>
        public static CoreHost Start(string serviceName)
        {
            lock (_lock)
            {
                if (_instance != null)
                    throw new InvalidOperationException($"{serviceName} already started");

                var cfg = LoadConfiguration(serviceName);
                _instance = new CoreHost(cfg);

                // Auto-dispose on app exit
                AppDomain.CurrentDomain.ProcessExit += (s, e) => _instance?.Dispose();

                return _instance;
            }
        }

        private static CoreConfig LoadConfiguration(string serviceName)
        {
            string sharedJsonPath = Path.Combine(
                AppContext.BaseDirectory,
                "..", "..", "..", "..", "SharedConfig"
            );
            sharedJsonPath = Path.GetFullPath(sharedJsonPath);

            var configRoot = new ConfigurationBuilder()
                .SetBasePath(sharedJsonPath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var coreSection = configRoot.GetSection("CoreConfig");
            var dbConn = coreSection.GetValue<string>("DatabaseConnectionString")
                ?? throw new InvalidOperationException("DatabaseConnectionString not found");
            var defaultLog = coreSection.GetValue<string>("DefaultLogPath")
                ?? throw new InvalidOperationException("DefaultLogPath not found");

            var rabbitSection = coreSection.GetSection("Rabbit");

            var rabbitCfg = new RabbitConfig(
                Host: rabbitSection.GetValue<string>("Host")
                    ?? throw new InvalidOperationException("Rabbit.Host missing"),

                Port: rabbitSection.GetValue<int>("Port"),

                Username: rabbitSection.GetValue<string>("Username")
                    ?? throw new InvalidOperationException("Rabbit.Username missing"),

                Password: rabbitSection.GetValue<string>("Password")
                    ?? throw new InvalidOperationException("Rabbit.Password missing")
            );

            var servicesSection = coreSection.GetSection("Services");
            var logPath = servicesSection.GetSection(serviceName).GetValue<string>("LogPath")
                ?? defaultLog;

            

            return new CoreConfig(
                ApplicationName: serviceName,
                Database: new DatabaseConfig(dbConn),
                LogPath: logPath,
                RabbitConfig: rabbitCfg
            );
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed) return;

                _bus?.Dispose();
                _db?.Dispose();
                _log?.Info("{AppName} stopped", _applicationName);

                _disposed = true;
                _instance = null;
            }
        }
    }
}
