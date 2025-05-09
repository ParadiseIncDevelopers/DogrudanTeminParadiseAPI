using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IMarketResearchJuryService
    {
        Task<MarketResearchJuryDto> CreateAsync(CreateMarketResearchJuryDto dto);
        Task<IEnumerable<MarketResearchJuryDto>> GetAllByEntryAsync(Guid entryId);
        Task<MarketResearchJuryDto> GetByIdAsync(Guid id);
        Task<MarketResearchJuryDto> UpdateAsync(Guid id, UpdateMarketResearchJuryDto dto);
        Task DeleteAsync(Guid id);
    }
}
