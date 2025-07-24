using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSInspectionAcceptanceNoteService
    {
        Task<OSInspectionAcceptanceNoteDto> CreateAsync(CreateOSInspectionAcceptanceNoteDto dto);
        Task<IEnumerable<OSInspectionAcceptanceNoteDto>> GetAllByEntryAsync(Guid entryId);
        Task<OSInspectionAcceptanceNoteDto?> GetByIdAsync(Guid id);
        Task<OSInspectionAcceptanceNoteDto?> UpdateAsync(Guid id, UpdateOSInspectionAcceptanceNoteDto dto);
        Task DeleteAsync(Guid id);
    }
}
