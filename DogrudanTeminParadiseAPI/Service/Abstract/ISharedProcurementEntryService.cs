using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISharedProcurementEntryService
    {
        Task<SharedProcurementEntryDto> CreateAsync(CreateSharedProcurementEntryDto dto);
        Task<IEnumerable<SharedProcurementEntryDto>> GetByUserAsync(Guid userId);
    }
}
