using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ISubAdministrationUnitService
    {
        Task<SubAdministrationUnitDto> CreateAsync(CreateSubAdministrationUnitDto dto);
        Task<IEnumerable<SubAdministrationUnitDto>> GetAllAsync();
        Task<SubAdministrationUnitDto> GetByIdAsync(Guid id);
        Task<SubAdministrationUnitDto> UpdateAsync(Guid id, UpdateSubAdministrationUnitDto dto);
        Task DeleteAsync(Guid id);
    }
}
