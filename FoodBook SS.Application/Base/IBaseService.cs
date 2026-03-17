using FoodBook_SS.Domain.Base;
namespace FoodBook_SS.Application.Base
{

    public interface IBaseService<TDto, TSave, TUpdate>
        where TDto : DtoBase
        where TSave : class
        where TUpdate : class
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> SaveAsync(TSave dto);
        Task<OperationResult> UpdateAsync(int id, TUpdate dto);
    }
}

