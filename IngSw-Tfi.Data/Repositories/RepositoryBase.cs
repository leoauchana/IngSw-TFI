using IngSw_Tfi.Domain.Repository;
using System.Data;

namespace IngSw_Tfi.Data.Repositories;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
{
        public abstract Task<List<TEntity>> GetAll();
        public abstract Task<TEntity?> GetById(int id);
        public abstract Task Add(TEntity entity);
        public abstract Task Update(TEntity entity);
        public abstract Task Delete(int id);
        protected abstract TEntity MapEntity(IDataRecord reader);
}