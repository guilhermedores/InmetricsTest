using Microsoft.EntityFrameworkCore.Storage;
using ShareLib;

public class UnitOfWork : IUnitOfWork
{
    private readonly InMemoryDbContext _context;

    public UnitOfWork(InMemoryDbContext context)
    {
        _context = context;
    }
    public IDbContextTransaction BeginTransaction() =>
        _context.Database.BeginTransaction();

    public bool Commit()
    {
        var result = _context.SaveChanges();
        return result > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
