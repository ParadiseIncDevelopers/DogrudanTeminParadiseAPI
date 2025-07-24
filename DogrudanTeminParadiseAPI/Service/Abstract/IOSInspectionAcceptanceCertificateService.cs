using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSInspectionAcceptanceCertificateService
    {
        Task<OSInspectionAcceptanceCertificateDto> CreateAsync(CreateOSInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllAsync();
        Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        Task<IEnumerable<OSInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds);
        Task<OSInspectionAcceptanceCertificateDto?> GetByIdAsync(Guid id);
        Task<OSInspectionAcceptanceCertificateDto?> UpdateAsync(Guid id, UpdateOSInspectionAcceptanceCertificateDto dto);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
