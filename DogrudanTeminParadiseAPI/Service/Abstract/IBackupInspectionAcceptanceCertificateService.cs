using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBackupInspectionAcceptanceCertificateService
    {
        Task<BackupInspectionAcceptanceCertificateDto> CreateAsync(CreateBackupInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllAsync();
        Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        Task<IEnumerable<BackupInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds);
        Task<BackupInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
