using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSProcurementEntryDocumentsService
    {
        Task<OSProcurementEntryDocumentsDto> CreateAsync(CreateOSProcurementEntryDocumentsDto dto);
        Task<IEnumerable<OSProcurementEntryDocumentsDto>> GetAllByEntryAsync(Guid entryId);
        Task<OSProcurementEntryDocumentsDto> GetByIdAsync(Guid id);
        Task<OSProcurementEntryDocumentsDto> UpdateAsync(Guid id, UpdateOSProcurementEntryDocumentsDto dto);
        Task DeleteAsync(Guid id);
    }
}
