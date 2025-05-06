using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IAssignedPersonnelService
    {
        Task<AssignedPersonnelDto> CreateAsync(CreateAssignedPersonnelDto dto);
        Task<AssignedPersonnelDto> UpdateAsync(Guid procurementEntryId, CreateAssignedPersonnelDto dto);
        Task<AssignedPersonnelDto> GetByEntryAsync(Guid procurementEntryId);
    }
}
