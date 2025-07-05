using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BackupOfferLetterService : IBackupOfferLetterService
    {
        private readonly MongoDBRepository<BackupOfferLetter> _repo;
        private readonly IMapper _mapper;

        public BackupOfferLetterService(MongoDBRepository<BackupOfferLetter> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BackupOfferLetterDto> CreateAsync(CreateBackupOfferLetterDto dto)
        {
            var entity = _mapper.Map<BackupOfferLetter>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<BackupOfferLetterDto>(entity);
        }

        public async Task<IEnumerable<BackupOfferLetterDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync()).Where(o => o.ProcurementEntryId == entryId);
            return list.Select(o => _mapper.Map<BackupOfferLetterDto>(o));
        }

        public async Task<BackupOfferLetterDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BackupOfferLetterDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Teklif mektubu bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}

