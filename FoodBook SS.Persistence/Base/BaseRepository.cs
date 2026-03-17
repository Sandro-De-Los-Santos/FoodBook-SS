using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FoodBook_SS.Persistence.Base
{
    public abstract class BaseRepositorycs<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly FoodBookDbContext _contex;
        protected DbSet<TEntity> Entity { get; set; }

        public BaseRepositorycs(FoodBookDbContext context)
        {
            _contex = context;
            Entity = context.Set<TEntity>();
        }

        public virtual async Task<OperationResult> SaveEntityAsync(TEntity entity)
        {
            var result = new OperationResult();
            try
            {
                Entity.Add(entity);
                await _contex.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public virtual async Task<OperationResult> UpdateEntityAsync(TEntity entity)
        {
            var result = new OperationResult();
            try
            {
                Entity.Update(entity);
                await _contex.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public virtual async Task<OperationResult> GetAllAsync(Expression<Func<TEntity, bool>> filter)
        {
            var result = new OperationResult();
            try
            {
                result.Data = await Entity.Where(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public virtual async Task<TEntity?> GetEntityByIdAsync(int id) => await Entity.FindAsync(id);

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter) =>
            await Entity.AnyAsync(filter);

        public virtual async Task<List<TEntity>> GetAllAsync() => await Entity.ToListAsync();
    }
}
