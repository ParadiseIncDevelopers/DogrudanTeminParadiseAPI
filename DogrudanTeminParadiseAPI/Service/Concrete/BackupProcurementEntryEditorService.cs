using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class BackupProcurementEntryEditorService : IBackupProcurementEntryEditorService
    {
        private readonly MongoDBRepository<BackupProcurementEntryEditor> _repo;
        private readonly IMapper _mapper;

        public BackupProcurementEntryEditorService(MongoDBRepository<BackupProcurementEntryEditor> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<BackupProcurementEntryEditorDto> CreateAsync(CreateBackupProcurementEntryEditorDto dto)
        {
            var entity = _mapper.Map<BackupProcurementEntryEditor>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<BackupProcurementEntryEditorDto>(entity);
        }

        public async Task<IEnumerable<BackupProcurementEntryEditorDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync()).Where(e => e.ProcurementEntryId == entryId);
            return list.Select(e => _mapper.Map<BackupProcurementEntryEditorDto>(e));
        }

        public async Task<BackupProcurementEntryEditorDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<BackupProcurementEntryEditorDto>(e);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Kayıt bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
