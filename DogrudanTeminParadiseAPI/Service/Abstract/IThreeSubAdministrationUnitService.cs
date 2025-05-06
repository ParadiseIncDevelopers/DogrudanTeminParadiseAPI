using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IThreeSubAdministrationUnitService
    {
        Task<ThreeSubAdministrationUnitDto> CreateAsync(CreateThreeSubAdministrationUnitDto dto);
        Task<IEnumerable<ThreeSubAdministrationUnitDto>> GetAllAsync();
        Task<ThreeSubAdministrationUnitDto> GetByIdAsync(Guid id);
        Task<ThreeSubAdministrationUnitDto> UpdateAsync(Guid id, UpdateThreeSubAdministrationUnitDto dto);
        Task DeleteAsync(Guid id);
    }
}
