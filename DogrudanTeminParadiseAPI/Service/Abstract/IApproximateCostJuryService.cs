using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IApproximateCostJuryService
    {
        Task<ApproximateCostJuryDto> CreateAsync(CreateApproximateCostJuryDto dto);
        Task<IEnumerable<ApproximateCostJuryDto>> GetAllByEntryAsync(Guid entryId);
        Task<ApproximateCostJuryDto> GetByIdAsync(Guid id);
        Task<ApproximateCostJuryDto> UpdateAsync(Guid id, UpdateApproximateCostJuryDto dto);
        Task DeleteAsync(Guid id);
    }
}
