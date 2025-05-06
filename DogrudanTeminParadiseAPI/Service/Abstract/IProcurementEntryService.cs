using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IProcurementEntryService
    {
        Task<ProcurementEntryDto> CreateAsync(CreateProcurementEntryDto dto);
        Task<IEnumerable<ProcurementEntryDto>> GetAllAsync();
        Task<ProcurementEntryDto> GetByIdAsync(Guid id);
        Task<ProcurementEntryDto> UpdateAsync(Guid id, UpdateProcurementEntryDto dto);
        Task DeleteAsync(Guid id);
    }
}
