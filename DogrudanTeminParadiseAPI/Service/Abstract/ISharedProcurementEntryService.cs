using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISharedProcurementEntryService
    {
        Task<SharedProcurementEntryDto> CreateAsync(CreateSharedProcurementEntryDto dto);
        Task<SharedProcurementEntryDto> GetByUserAsync(Guid userId, Guid procurementEntryId);
    }
}
