using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IProductItemService
    {
        Task<ProductItemDto> CreateAsync(CreateProductItemDto dto);
        Task<IEnumerable<ProductItemDto>> AddMassAsync(List<CreateProductItemDto> dtos);
        Task<IEnumerable<ProductItemDto>> GetAllAsync();
        Task<ProductItemDto> GetByIdAsync(Guid id);
        Task<ProductItemDto> UpdateAsync(Guid id, UpdateProductItemDto dto);
        Task DeleteAsync(Guid id);
    }
}
