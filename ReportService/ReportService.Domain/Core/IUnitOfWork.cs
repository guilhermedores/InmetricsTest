using Microsoft.EntityFrameworkCore.Storage;

namespace ShareLib
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContextTransaction BeginTransaction();
        bool Commit();
    }
}
