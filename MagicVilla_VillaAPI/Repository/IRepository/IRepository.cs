using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate = null, bool tracked = true);
        // 38, When working with repository, convert(map) Villa to DTOs and vice versa will be done inside APIController
        // 38, Data layer will directly interact with the Villa entity.
        Task CreateAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);
        Task SaveAsync();
    }
}
