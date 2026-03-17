using FoodBook_SS.Domain.Base;
using System.Linq.Expressions;
namespace FoodBook_SS.Domain.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<OperationResult> SaveEntityAsync(TEntity entity);
        Task<OperationResult> UpdateEntityAsync(TEntity entity);
        Task<TEntity?> GetEntityByIdAsync(int id);
        Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<List<TEntity>> GetAllAsync();
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}
