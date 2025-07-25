using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSApproximateCostJuryService : IOSApproximateCostJuryService
    {
        private readonly MongoDBRepository<OSApproximateCostJury> _repo;
        private readonly IMapper _mapper;

        public OSApproximateCostJuryService(
            MongoDBRepository<OSApproximateCostJury> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSApproximateCostJuryDto> CreateAsync(CreateOSApproximateCostJuryDto dto)
        {
            var entity = _mapper.Map<OSApproximateCostJury>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSApproximateCostJuryDto>(entity);
        }

        public async Task<IEnumerable<OSApproximateCostJuryDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(j => j.OneSourceProcurementEntryId == entryId);
            return list.Select(_mapper.Map<OSApproximateCostJuryDto>);
        }

        public async Task<OSApproximateCostJuryDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSApproximateCostJuryDto>(e);
        }

        public async Task<OSApproximateCostJuryDto?> UpdateAsync(Guid id, UpdateOSApproximateCostJuryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.UserIds = dto.UserIds;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<OSApproximateCostJuryDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Jüri kaydı bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
