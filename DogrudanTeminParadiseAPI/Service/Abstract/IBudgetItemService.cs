using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBudgetItemService
    {
        Task<BudgetItemDto> CreateAsync(CreateBudgetItemDto dto);
        Task<IEnumerable<BudgetItemDto>> AddMassAsync(List<CreateBudgetItemDto> dtos);
        Task<IEnumerable<BudgetItemDto>> GetAllAsync();
        Task<BudgetItemDto> GetByIdAsync(Guid id);
        Task<BudgetItemDto> UpdateAsync(Guid id, UpdateBudgetItemDto dto);
        Task DeleteAsync(Guid id);
    }
}
