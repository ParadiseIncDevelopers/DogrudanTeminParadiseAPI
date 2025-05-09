using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IInspectionAcceptanceJuryService
    {
        Task<InspectionAcceptanceJuryDto> CreateAsync(CreateInspectionAcceptanceJuryDto dto);
        Task<IEnumerable<InspectionAcceptanceJuryDto>> GetAllByEntryAsync(Guid entryId);
        Task<InspectionAcceptanceJuryDto> GetByIdAsync(Guid id);
        Task<InspectionAcceptanceJuryDto> UpdateAsync(Guid id, UpdateInspectionAcceptanceJuryDto dto);
        Task DeleteAsync(Guid id);
    }
}
