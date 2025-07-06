using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IAdditionalInspectionAcceptanceService
    {
        Task<AdditionalInspectionAcceptanceCertificateDto> CreateAsync(CreateAdditionalInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllAsync();
        Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds);
        Task<AdditionalInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id);
        Task<AdditionalInspectionAcceptanceCertificateDto> UpdateAsync(Guid id, UpdateAdditionalInspectionAcceptanceCertificateDto dto);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
