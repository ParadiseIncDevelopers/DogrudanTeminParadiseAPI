using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISubInspectionAcceptanceJuryService
    {
        Task<SubInspectionAcceptanceJuryDto> CreateAsync(CreateSubInspectionAcceptanceJuryDto dto);
        Task<IEnumerable<SubInspectionAcceptanceJuryDto>> GetAllAsync();
        Task<SubInspectionAcceptanceJuryDto> GetByIdAsync(Guid id);
        Task<SubInspectionAcceptanceJuryDto> UpdateAsync(Guid id, UpdateSubInspectionAcceptanceJuryDto dto);
        Task DeleteAsync(Guid id);
    }
}
