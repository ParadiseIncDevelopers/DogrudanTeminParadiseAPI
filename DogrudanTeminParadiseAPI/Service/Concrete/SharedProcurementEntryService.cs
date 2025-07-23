using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class SharedProcurementEntryService : ISharedProcurementEntryService
    {
        private readonly MongoDBRepository<SharedProcurementEntry> _repo;
        private readonly IMapper _mapper;

        public SharedProcurementEntryService(MongoDBRepository<SharedProcurementEntry> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<SharedProcurementEntryDto> CreateAsync(CreateSharedProcurementEntryDto dto)
        {
            var entity = _mapper.Map<SharedProcurementEntry>(dto);
            entity.Id = Guid.NewGuid();
            entity.SharingDate = DateTime.UtcNow;
            await _repo.InsertAsync(entity);
            return _mapper.Map<SharedProcurementEntryDto>(entity);
        }

        public async Task<SharedProcurementEntryDto> GetByUserAsync(Guid userId, Guid procurementEntryId)
        {
            var shared = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.ProcurementSharerUserId == userId && x.ProcurementId == procurementEntryId);

            if (shared != null)
            {
                return _mapper.Map<SharedProcurementEntryDto>(shared);
            }
            else 
            {
                return new SharedProcurementEntryDto();
            } 
        }

        public async Task DeleteUserFromSharersAsync(Guid procurementId, Guid userId)
        {
            var shared = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.ProcurementId == procurementId && x.SharedToUserIds.Contains(userId));

            if (shared == null)
                throw new KeyNotFoundException("Paylaşım bulunamadı.");

            shared.SharedToUserIds.Remove(userId);
            await _repo.UpdateAsync(shared.Id, shared);
        }

        public async Task<SharedProcurementEntryDto> UpdateSharedToIdsAsync(Guid procurementId, List<Guid> sharedToUserIds)
        {
            var shared = (await _repo.GetAllAsync())
                .FirstOrDefault(x => x.ProcurementId == procurementId);

            if (shared == null)
                throw new KeyNotFoundException("Paylaşım bulunamadı.");

            shared.SharedToUserIds = sharedToUserIds ?? new();
            await _repo.UpdateAsync(shared.Id, shared);

            return _mapper.Map<SharedProcurementEntryDto>(shared);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = (await _repo.GetAllAsync()).FirstOrDefault(x => x.ProcurementId == id) ?? throw new KeyNotFoundException("Paylaşım bulunamadı.");
            await _repo.DeleteAsync(existing.Id);
        }
    }
}
