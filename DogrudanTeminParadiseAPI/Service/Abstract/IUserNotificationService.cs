using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IUserNotificationService
    {
        Task<UserNotificationDto> CreateAsync(CreateUserNotificationDto dto);
        Task<IEnumerable<UserNotificationDto>> GetAsync(UserNotificationQueryDto query);
        Task<UserNotificationDto> UpdateAsync(Guid id, UpdateUserNotificationDto dto);
        Task DeleteAsync(Guid id);
    }
}
