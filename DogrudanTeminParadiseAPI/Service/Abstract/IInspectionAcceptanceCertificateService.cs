using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IInspectionAcceptanceCertificateService
    {
        Task<InspectionAcceptanceCertificateDto> CreateAsync(CreateInspectionAcceptanceCertificateDto dto);
        Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllAsync();
        /// <summary>Returns all certificates for permitted entries.</summary>
        Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId);
        /// <summary>Returns certificates by entry only if entryId is in permittedEntryIds.</summary>
        Task<IEnumerable<InspectionAcceptanceCertificateDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds);
        Task<InspectionAcceptanceCertificateDto> GetByIdAsync(Guid id);
        Task<InspectionAcceptanceCertificateDto> UpdateAsync(Guid id, UpdateInspectionAcceptanceCertificateDto dto);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
