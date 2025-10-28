namespace IngSw_Tfi.Domain.Repository;

public interface IRepository<TEntity>
{
    Task<List<TEntity>?> GetAll();
    Task<TEntity?> GetById(int id);
    Task Add(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(int id);
}
