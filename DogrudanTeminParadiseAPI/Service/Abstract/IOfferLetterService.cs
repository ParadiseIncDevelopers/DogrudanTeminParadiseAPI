using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOfferLetterService
    {
        Task<OfferLetterDto> CreateAsync(CreateOfferLetterDto dto);
        Task<IEnumerable<OfferLetterDto>> GetAllAsync();
        Task<IEnumerable<OfferLetterDto>> GetAllAsync(IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<OfferLetterDto>> GetAllByEntryAsync(Guid procurementEntryId);
        Task<IEnumerable<OfferLetterDto>> GetAllByEntryAsync(Guid procurementEntryId, IEnumerable<Guid> permittedEntryIds);
        Task<IEnumerable<OfferLetterDto>> UpdateItemsByEntryAsync(Guid procurementEntryId, UpdateOfferItemsByEntryDto dto);
        Task<OfferLetterDto> GetByIdAsync(Guid id);
        Task<OfferLetterDto> UpdateAsync(Guid id, UpdateOfferLetterDto dto);
        Task DeleteAsync(Guid id, Guid userId);
    }
}
