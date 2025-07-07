using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IDecisionNumbersService
    {
        Task<DecisionNumbersDto> CreateAsync(CreateDecisionNumbersDto dto);
        Task<IEnumerable<DecisionNumbersDto>> GetAllAsync();
        Task<DecisionNumbersDto?> GetByIdAsync(Guid id);
        Task<DecisionNumbersDto?> UpdateAsync(Guid id, UpdateDecisionNumbersDto dto);
        Task DeleteAsync(Guid id);
    }
}
