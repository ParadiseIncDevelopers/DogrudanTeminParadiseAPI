using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSSharedProcurementEntryService : IOSSharedProcurementEntryService
    {
        private readonly MongoDBRepository<OSSharedProcurementEntry> _repo;
        private readonly IMapper _mapper;

        public OSSharedProcurementEntryService(MongoDBRepository<OSSharedProcurementEntry> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSSharedProcurementEntryDto> CreateAsync(CreateOSSharedProcurementEntryDto dto)
        {
            var entity = _mapper.Map<OSSharedProcurementEntry>(dto);
            entity.Id = Guid.NewGuid();
            entity.SharingDate = DateTime.UtcNow;
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSSharedProcurementEntryDto>(entity);
        }

        public async Task<OSSharedProcurementEntryDto> GetByUserAsync(Guid userId, Guid entryId)
        {
            var shared = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.ProcurementSharerUserId == userId && x.OneSourceProcurementEntryId == entryId);
            return shared == null ? new OSSharedProcurementEntryDto() : _mapper.Map<OSSharedProcurementEntryDto>(shared);
        }

        public async Task DeleteUserFromSharersAsync(Guid entryId, Guid userId)
        {
            var shared = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.OneSourceProcurementEntryId == entryId && x.SharedToUserIds.Contains(userId));
            if (shared == null)
                throw new KeyNotFoundException("Paylaşım bulunamadı.");
            shared.SharedToUserIds.Remove(userId);
            await _repo.UpdateAsync(shared.Id, shared);
        }

        public async Task<OSSharedProcurementEntryDto> UpdateSharedToIdsAsync(Guid entryId, List<Guid> sharedToUserIds)
        {
            var shared = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.OneSourceProcurementEntryId == entryId);
            if (shared == null)
                throw new KeyNotFoundException("Paylaşım bulunamadı.");
            shared.SharedToUserIds = sharedToUserIds ?? new();
            await _repo.UpdateAsync(shared.Id, shared);
            return _mapper.Map<OSSharedProcurementEntryDto>(shared);
        }

        public async Task DeleteAsync(Guid shareEntryId)
        {
            var existing = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.OneSourceProcurementEntryId == shareEntryId) ?? throw new KeyNotFoundException("Paylaşım bulunamadı.");
            await _repo.DeleteAsync(existing.Id);
        }
    }
}
