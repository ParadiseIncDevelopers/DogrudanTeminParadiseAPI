using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IInspectionAcceptanceNoteService
    {
        Task<InspectionAcceptanceNoteDto> CreateAsync(CreateInspectionAcceptanceNoteDto dto);
        Task<IEnumerable<InspectionAcceptanceNoteDto>> GetAllByEntryAsync(Guid procurementEntryId);
        Task<InspectionAcceptanceNoteDto?> GetByIdAsync(Guid id);
        Task<InspectionAcceptanceNoteDto?> UpdateAsync(Guid id, UpdateInspectionAcceptanceNoteDto dto);
        Task DeleteAsync(Guid id);
    }
}
