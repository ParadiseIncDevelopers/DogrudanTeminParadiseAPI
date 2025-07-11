using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task DeleteAsync(Guid id);
        Task<NotificationDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task<IEnumerable<NotificationDto>> GetAllByUserIdAsync(Guid userId);

        Task<NotificationDto> MarkIsReadAsync(Guid id);
    }
}
