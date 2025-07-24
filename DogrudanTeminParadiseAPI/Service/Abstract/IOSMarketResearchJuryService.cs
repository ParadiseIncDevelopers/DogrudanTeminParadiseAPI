using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSMarketResearchJuryService
    {
        Task<OSMarketResearchJuryDto> CreateAsync(CreateOSMarketResearchJuryDto dto);
        Task<IEnumerable<OSMarketResearchJuryDto>> GetAllByEntryAsync(Guid entryId);
        Task<OSMarketResearchJuryDto?> GetByIdAsync(Guid id);
        Task<OSMarketResearchJuryDto?> UpdateAsync(Guid id, UpdateOSMarketResearchJuryDto dto);
        Task DeleteAsync(Guid id);
    }
}
