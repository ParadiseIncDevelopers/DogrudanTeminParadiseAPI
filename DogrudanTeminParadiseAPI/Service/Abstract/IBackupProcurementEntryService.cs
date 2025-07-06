using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBackupProcurementEntryService
    {
        Task<BackupProcurementEntryDto> CreateAsync(CreateBackupProcurementEntryDto dto);
        Task<IEnumerable<BackupProcurementEntryDto>> GetAllAsync();
        Task<BackupProcurementEntryDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
