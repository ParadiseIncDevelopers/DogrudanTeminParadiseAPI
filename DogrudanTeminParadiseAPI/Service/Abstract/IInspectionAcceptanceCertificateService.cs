using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IInspectionAcceptanceCertificateService
    {
        Task<InspectionAcceptanceCertificateDto> CreateAsync(CreateInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        Task<InspectionAcceptanceCertificateDto> GetByIdAsync(Guid id);
        Task<InspectionAcceptanceCertificateDto> UpdateAsync(Guid id, UpdateInspectionAcceptanceCertificateDto dto);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllAsync();
    }
}
