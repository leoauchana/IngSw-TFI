namespace IngSw_Tfi.Domain.Repository;

public interface IRepository<TEntity>
{
    Task<TEntity?> GetById(int id);
    Task Add(TEntity entity);
}
