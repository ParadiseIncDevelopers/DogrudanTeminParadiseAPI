using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<IEnumerable<CategoryDto>> AddMassAsync(List<CreateCategoryDto> dtos);
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(Guid id);
        Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto);
        Task DeleteAsync(Guid id);
    }
}
