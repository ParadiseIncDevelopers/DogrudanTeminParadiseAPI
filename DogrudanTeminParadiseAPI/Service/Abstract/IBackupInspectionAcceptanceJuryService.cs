using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBackupInspectionAcceptanceJuryService
    {
        Task<BackupInspectionAcceptanceJuryDto> CreateAsync(CreateBackupInspectionAcceptanceJuryDto dto);
        Task<IEnumerable<BackupInspectionAcceptanceJuryDto>> GetAllByEntryAsync(Guid entryId);
        Task<BackupInspectionAcceptanceJuryDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
