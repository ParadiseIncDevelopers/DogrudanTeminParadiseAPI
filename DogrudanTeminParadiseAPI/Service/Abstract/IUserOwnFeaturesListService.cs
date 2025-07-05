using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IUserOwnFeaturesListService
    {
        Task<UserOwnFeaturesListDto> CreateAsync(CreateUserOwnFeaturesListDto dto);
        Task<IEnumerable<UserOwnFeaturesListDto>> GetAllAsync();
        Task<IEnumerable<UserOwnFeaturesListDto>> GetByUserIdAsync(Guid userId);
        Task<UserOwnFeaturesListDto> GetByIdAsync(Guid id);
        Task<UserOwnFeaturesListDto> UpdateAsync(Guid id, UpdateUserOwnFeaturesListDto dto);
        Task DeleteAsync(Guid id);
    }
}
