using DogrudanTeminParadiseAPI.Dto;

namespace DogrudanTeminParadiseAPI.Service.Abstract
{
    public interface IBackupOfferLetterService
    {
        Task<BackupOfferLetterDto> CreateAsync(CreateBackupOfferLetterDto dto);
        Task<IEnumerable<BackupOfferLetterDto>> GetAllByEntryAsync(Guid entryId);
        Task<BackupOfferLetterDto> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}

