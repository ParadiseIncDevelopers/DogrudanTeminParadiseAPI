using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSAdditionalInspectionAcceptanceService
    {
        Task<OSAdditionalInspectionAcceptanceCertificateDto> CreateAsync(CreateOSAdditionalInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync();
        Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        Task<IEnumerable<OSAdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds);
        Task<OSAdditionalInspectionAcceptanceCertificateDto?> GetByIdAsync(Guid id);
        Task<OSAdditionalInspectionAcceptanceCertificateDto?> UpdateAsync(Guid id, UpdateOSAdditionalInspectionAcceptanceCertificateDto dto);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
