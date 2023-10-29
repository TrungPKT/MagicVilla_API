using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    // 38, Real world application Controller does not interract directly with DB. -> Repsotory as middle layer to manipulate data in DB
    public interface IVillaRepository
    {
        Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> predicate = null);
        Task<Villa> GetAsync(Expression<Func<Villa, bool>> predicate = null, bool tracked = true);
        // 38, When working with repository, convert(map) Villa to DTOs and vice versa will be done inside APIController
        // 38, Data layer will directly interact with the Villa entity.
        Task CreateAsync(Villa entity);
        Task UpdateAsync(Villa entity);
        Task RemoveAsync(Villa entity);
        Task SaveAsync();
    }
}
