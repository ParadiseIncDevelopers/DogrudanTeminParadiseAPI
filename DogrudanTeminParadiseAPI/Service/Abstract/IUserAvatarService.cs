using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IUserAvatarService
    {
        Task<UserAvatarDto> CreateAsync(CreateUserAvatarDto dto);
        Task<UserAvatarDto?> GetByUserOrAdminIdAsync(Guid userOrAdminId);
        Task<UserAvatarDto?> UpdateAsync(Guid userOrAdminId, UpdateUserAvatarDto dto);
    }
}
