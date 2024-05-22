namespace ShareLib;

public interface IGenericRepository<TEntity> where TEntity : Entity
{
    IQueryable<TEntity> GetAll();

    Task<TEntity?> GetById(Guid id);

    Task Insert(TEntity obj);

    Task Delete(Guid id);

    Task Delete(TEntity obj);

    Task Update(TEntity obj);

    Task Save();
}
