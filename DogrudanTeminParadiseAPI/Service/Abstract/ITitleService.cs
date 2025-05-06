using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ITitleService
    {
        Task<TitleDto> CreateAsync(CreateTitleDto dto);
        Task<IEnumerable<TitleDto>> GetAllAsync();
        Task<TitleDto> GetByIdAsync(Guid id);
        Task<TitleDto> UpdateAsync(Guid id, UpdateTitleDto dto);
        Task DeleteAsync(Guid id);
    }
}
