using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IAdministrationUnitService
    {
        Task<AdministrationUnitDto> CreateAsync(CreateAdministrationUnitDto dto);
        Task<IEnumerable<AdministrationUnitDto>> GetAllAsync();
        Task<AdministrationUnitDto> GetByIdAsync(Guid id);
        Task<AdministrationUnitDto> UpdateAsync(Guid id, UpdateAdministrationUnitDto dto);
        Task DeleteAsync(Guid id);
    }
}
