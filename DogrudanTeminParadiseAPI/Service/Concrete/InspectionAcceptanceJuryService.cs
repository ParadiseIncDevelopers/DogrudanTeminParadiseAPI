using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class InspectionAcceptanceJuryService : IInspectionAcceptanceJuryService
    {
        private readonly MongoDBRepository<InspectionAcceptanceJury> _repo;
        private readonly MongoDBRepository<BackupInspectionAcceptanceJury> _backupRepo;
        private readonly IMapper _mapper;

        public InspectionAcceptanceJuryService(
            MongoDBRepository<InspectionAcceptanceJury> repo,
            MongoDBRepository<BackupInspectionAcceptanceJury> backupRepo,
            IMapper mapper)
        {
            _repo = repo;
            _backupRepo = backupRepo;
            _mapper = mapper;
        }

        public async Task<InspectionAcceptanceJuryDto> CreateAsync(CreateInspectionAcceptanceJuryDto dto)
        {
            var entity = _mapper.Map<InspectionAcceptanceJury>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<InspectionAcceptanceJuryDto>(entity);
        }

        public async Task<IEnumerable<InspectionAcceptanceJuryDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(j => j.ProcurementEntryId == entryId);
            return list.Select(j => _mapper.Map<InspectionAcceptanceJuryDto>(j));
        }

        public async Task<InspectionAcceptanceJuryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<InspectionAcceptanceJuryDto>(e);
        }

        public async Task<InspectionAcceptanceJuryDto> UpdateAsync(Guid id, UpdateInspectionAcceptanceJuryDto dto)
        {
            var allInspectionsJuries = await _repo.GetAllAsync();
            if (allInspectionsJuries == null) 
                return null;
            var existing = allInspectionsJuries.FirstOrDefault(x => x.InspectionAcceptanceJuryId == id) ?? throw new KeyNotFoundException("Jüriler bulunamadı");

            existing.UserIds = dto.UserIds;
            await _repo.UpdateAsync(existing.Id, existing);
            return _mapper.Map<InspectionAcceptanceJuryDto>(existing);
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Jüri kaydı bulunamadı.");
            var backup = _mapper.Map<BackupInspectionAcceptanceJury>(existing);
            backup.RemovedByUserId = userId;
            backup.RemovingDate = DateTime.UtcNow;
            await _backupRepo.InsertAsync(backup);
            await _repo.DeleteAsync(id);
        }
    }
}
