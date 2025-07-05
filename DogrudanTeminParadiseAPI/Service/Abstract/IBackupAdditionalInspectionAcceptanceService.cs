using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBackupAdditionalInspectionAcceptanceService
    {
        Task<BackupAdditionalInspectionAcceptanceCertificateDto> CreateAsync(CreateBackupAdditionalInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync();
        Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        Task<IEnumerable<BackupAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds);
        Task<BackupAdditionalInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
