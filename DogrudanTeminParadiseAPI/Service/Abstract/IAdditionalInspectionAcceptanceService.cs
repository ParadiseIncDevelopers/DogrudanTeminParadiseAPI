using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IAdditionalInspectionAcceptanceService
    {
        Task<AdditionalInspectionAcceptanceCertificateDto> CreateAsync(CreateAdditionalInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        Task<AdditionalInspectionAcceptanceCertificateDto> GetByIdAsync(Guid id);
        Task<IEnumerable<AdditionalInspectionAcceptanceCertificateDto>> GetAllAsync();
        Task<AdditionalInspectionAcceptanceCertificateDto> UpdateAsync(Guid id, UpdateAdditionalInspectionAcceptanceCertificateDto dto);
        Task DeleteAsync(Guid id);
    }
}
