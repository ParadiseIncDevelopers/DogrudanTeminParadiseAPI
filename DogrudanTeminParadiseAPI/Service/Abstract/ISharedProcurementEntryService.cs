using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISharedProcurementEntryService
    {
        Task<SharedProcurementEntryDto> CreateAsync(CreateSharedProcurementEntryDto dto);
        Task<SharedProcurementEntryDto> GetByUserAsync(Guid userId, Guid procurementEntryId);
        Task DeleteUserFromSharersAsync(Guid procurementId, Guid userId);
        Task<SharedProcurementEntryDto> UpdateSharedToIdsAsync(Guid procurementId, List<Guid> sharedToUserIds);
    }
}
