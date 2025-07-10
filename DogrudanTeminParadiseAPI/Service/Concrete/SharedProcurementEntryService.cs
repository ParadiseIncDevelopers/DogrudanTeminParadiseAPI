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
            return _mapper.Map<SharedProcurementEntryDto>(shared);
        }
    }
}
