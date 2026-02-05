using System.Data.Common;

namespace HSM_CommonCS.Database
{
    public interface IDbSessionFactory : IDisposable
    {
        DbConnection Open();
    }
}
