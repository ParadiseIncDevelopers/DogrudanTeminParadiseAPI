using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSProcurementEntryService
    {
        Task<OSProcurementEntryDto> CreateAsync(CreateOSProcurementEntryDto dto);
        Task<IEnumerable<OSProcurementEntryDto>> GetAllAsync();
        Task<OSProcurementEntryDto?> GetByIdAsync(Guid id);
        Task<OSProcurementEntryDto?> UpdateAsync(Guid id, UpdateOSProcurementEntryDto dto);
        Task DeleteAsync(Guid id);
    }
}
