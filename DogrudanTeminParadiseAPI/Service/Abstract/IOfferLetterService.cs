using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IOfferLetterService
    {
        Task<OfferLetterDto> CreateAsync(CreateOfferLetterDto dto);
        Task<IEnumerable<OfferLetterDto>> GetAllByEntryAsync(Guid procurementEntryId);
        Task<OfferLetterDto> GetByIdAsync(Guid id);
        Task<IEnumerable<OfferLetterDto>> GetAllAsync();
        Task<OfferLetterDto> UpdateAsync(Guid id, UpdateOfferLetterDto dto);
        Task DeleteAsync(Guid id);
    }
}
