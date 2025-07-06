using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBackupProcurementEntryEditorService
    {
        Task<BackupProcurementEntryEditorDto> CreateAsync(CreateBackupProcurementEntryEditorDto dto);
        Task<IEnumerable<BackupProcurementEntryEditorDto>> GetAllByEntryAsync(Guid entryId);
        Task<BackupProcurementEntryEditorDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
