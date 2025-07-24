using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOSOfferLetterService
    {
        Task<OSOfferLetterDto> CreateAsync(CreateOSOfferLetterDto dto);
        Task<IEnumerable<OSOfferLetterDto>> GetAllAsync();
        Task<IEnumerable<OSOfferLetterDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<OSOfferLetterDto>> GetAllByEntryAsync(Guid entryId);
        Task<IEnumerable<OSOfferLetterDto>> GetAllByEntryAsync(Guid entryId, IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<OSOfferLetterDto>> UpdateItemsByEntryAsync(Guid entryId, UpdateOSOfferItemsByEntryDto dto);
        Task<OSOfferLetterDto> GetByIdAsync(Guid id);
        Task<OSOfferLetterDto> UpdateAsync(Guid id, UpdateOSOfferLetterDto dto);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
