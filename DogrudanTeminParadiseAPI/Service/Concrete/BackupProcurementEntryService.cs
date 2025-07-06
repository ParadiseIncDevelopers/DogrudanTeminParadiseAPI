using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BackupProcurementEntryService : IBackupProcurementEntryService
    {
        private readonly MongoDBRepository<BackupProcurementEntry> _repo;
        private readonly IMapper _mapper;

        public BackupProcurementEntryService(MongoDBRepository<BackupProcurementEntry> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BackupProcurementEntryDto> CreateAsync(CreateBackupProcurementEntryDto dto)
        {
            var entity = _mapper.Map<BackupProcurementEntry>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<BackupProcurementEntryDto>(entity);
        }

        public async Task<IEnumerable<BackupProcurementEntryDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => _mapper.Map<BackupProcurementEntryDto>(e));
        }

        public async Task<BackupProcurementEntryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BackupProcurementEntryDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
