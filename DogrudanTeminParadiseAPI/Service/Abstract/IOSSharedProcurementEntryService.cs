using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSSharedProcurementEntryService
    {
        Task<OSSharedProcurementEntryDto> CreateAsync(CreateOSSharedProcurementEntryDto dto);
        Task<OSSharedProcurementEntryDto> GetByUserAsync(Guid userId, Guid entryId);
        Task DeleteUserFromSharersAsync(Guid entryId, Guid userId);
        Task<OSSharedProcurementEntryDto> UpdateSharedToIdsAsync(Guid entryId, List<Guid> sharedToUserIds);
        Task DeleteAsync(Guid shareEntryId);
    }
}
