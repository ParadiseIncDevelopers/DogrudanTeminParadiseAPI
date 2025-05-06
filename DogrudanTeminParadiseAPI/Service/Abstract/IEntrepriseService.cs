using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IEntrepriseService
    {
        Task<EntrepriseDto> CreateAsync(CreateEntrepriseDto dto);
        Task<IEnumerable<EntrepriseDto>> GetAllAsync();
        Task<EntrepriseDto> GetByIdAsync(Guid id);
        Task<EntrepriseDto> UpdateAsync(Guid id, UpdateEntrepriseDto dto);
        Task DeleteAsync(Guid id);
    }
}
