using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BackupInspectionAcceptanceJuryService : IBackupInspectionAcceptanceJuryService
    {
        private readonly MongoDBRepository<BackupInspectionAcceptanceJury> _repo;
        private readonly IMapper _mapper;

        public BackupInspectionAcceptanceJuryService(MongoDBRepository<BackupInspectionAcceptanceJury> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BackupInspectionAcceptanceJuryDto> CreateAsync(CreateBackupInspectionAcceptanceJuryDto dto)
        {
            var entity = _mapper.Map<BackupInspectionAcceptanceJury>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<BackupInspectionAcceptanceJuryDto>(entity);
        }

        public async Task<IEnumerable<BackupInspectionAcceptanceJuryDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync()).Where(j => j.ProcurementEntryId == entryId);
            return list.Select(j => _mapper.Map<BackupInspectionAcceptanceJuryDto>(j));
        }

        public async Task<BackupInspectionAcceptanceJuryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BackupInspectionAcceptanceJuryDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Jüri kaydı bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
