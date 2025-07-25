using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSProcurementEntryService : IOSProcurementEntryService
    {
        private readonly MongoDBRepository<OSProcurementEntry> _repo;
        private readonly IMapper _mapper;

        public OSProcurementEntryService(MongoDBRepository<OSProcurementEntry> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSProcurementEntryDto> CreateAsync(CreateOSProcurementEntryDto dto)
        {
            var entity = _mapper.Map<OSProcurementEntry>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSProcurementEntryDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<OSProcurementEntryDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(_mapper.Map<OSProcurementEntryDto>);

        public async Task<OSProcurementEntryDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSProcurementEntryDto>(e);
        }

        public async Task<OSProcurementEntryDto?> UpdateAsync(Guid id, UpdateOSProcurementEntryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<OSProcurementEntryDto>(existing);
        }
    }
}
