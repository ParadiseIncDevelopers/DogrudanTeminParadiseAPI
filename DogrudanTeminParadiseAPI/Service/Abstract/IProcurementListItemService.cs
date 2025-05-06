using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IProcurementListItemService
    {
        Task<ProcurementListItemDto> CreateAsync(CreateProcurementListItemDto dto);
        Task<IEnumerable<ProcurementListItemDto>> GetAllByEntryAsync(Guid procurementEntryId);
        Task<ProcurementListItemDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
