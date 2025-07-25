using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSApproximateCostJuryService
    {
        Task<OSApproximateCostJuryDto> CreateAsync(CreateOSApproximateCostJuryDto dto);
        Task<IEnumerable<OSApproximateCostJuryDto>> GetAllByEntryAsync(Guid entryId);
        Task<OSApproximateCostJuryDto?> GetByIdAsync(Guid id);
        Task<OSApproximateCostJuryDto?> UpdateAsync(Guid id, UpdateOSApproximateCostJuryDto dto);
        Task DeleteAsync(Guid id);
    }
}
