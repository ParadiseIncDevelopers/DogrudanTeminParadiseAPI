using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBackupUserNotificationService
    {
        Task<BackupUserNotificationDto> CreateAsync(CreateBackupUserNotificationDto dto);
        Task<IEnumerable<BackupUserNotificationDto>> GetAllAsync();
        Task<BackupUserNotificationDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
