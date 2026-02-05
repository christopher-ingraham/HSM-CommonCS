using HSM_CommonCS.Database;

namespace HSM_CommonCS.Core
{
    /// <summary>
    /// Base class for application services providing convenient access to logging and database.
    /// Inherit from this for your business logic classes.
    /// </summary>
    public abstract class AppService
    {
        /// <summary>Gets the application logger</summary>
        protected static ILog Log => CoreHost.Log;

        /// <summary>Gets the database session factory</summary>
        protected static IDbSessionFactory Db => CoreHost.Db;
    }
}
