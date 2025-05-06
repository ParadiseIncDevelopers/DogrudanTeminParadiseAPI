using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IUnitService
    {
        Task<UnitDto> CreateAsync(CreateUnitDto dto);
        Task<IEnumerable<UnitDto>> GetAllAsync();
        Task<UnitDto> GetByIdAsync(Guid id);
        Task<UnitDto> UpdateAsync(Guid id, UpdateUnitDto dto);
        Task DeleteAsync(Guid id);
    }
}
