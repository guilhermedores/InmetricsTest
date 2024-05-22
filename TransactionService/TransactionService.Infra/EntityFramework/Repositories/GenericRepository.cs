using Microsoft.EntityFrameworkCore;
using ShareLib;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : Entity
{
    internal InMemoryDbContext context;
    internal DbSet<TEntity> dbSet;

    public GenericRepository(InMemoryDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> GetAll()
    {
        return dbSet;         
    }

    public async Task<TEntity?> GetById(Guid id)
    {
        TEntity? obj = await dbSet.FindAsync(id);
        if (obj != null)
            return obj;
        return null;
    }

    public async Task Insert(TEntity entity)
    {
        await dbSet.AddAsync(entity);
    }

    public async Task Delete(Guid id)
    {
        TEntity? obj = await dbSet.FindAsync(id);
        if (obj != null)
            await Delete(obj);
    }

    public async Task Delete(TEntity obj)
    {
        await Task.Run(() =>
        {
            if (context.Entry(obj).State == EntityState.Detached)
            {
                dbSet.Attach(obj);
            }
            dbSet.Remove(obj);
        });
    }

    public async Task Update(TEntity obj)
    {
        await Task.Run(() =>
        {
            dbSet.Attach(obj);
            context.Entry(obj).State = EntityState.Modified;
        });
    }

    public async Task Save()
    {
        await context.SaveChangesAsync();
    }
}