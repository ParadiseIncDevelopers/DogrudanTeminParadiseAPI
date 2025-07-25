using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class OSMarketResearchJuryService : IOSMarketResearchJuryService
    {
        private readonly MongoDBRepository<OSMarketResearchJury> _repo;
        private readonly IMapper _mapper;

        public OSMarketResearchJuryService(MongoDBRepository<OSMarketResearchJury> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<OSMarketResearchJuryDto> CreateAsync(CreateOSMarketResearchJuryDto dto)
        {
            var entity = _mapper.Map<OSMarketResearchJury>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<OSMarketResearchJuryDto>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
        }

        public async Task<IEnumerable<OSMarketResearchJuryDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(j => j.OneSourceProcurementEntryId == entryId);
            return list.Select(_mapper.Map<OSMarketResearchJuryDto>);
        }

        public async Task<OSMarketResearchJuryDto?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<OSMarketResearchJuryDto>(e);
        }

        public async Task<OSMarketResearchJuryDto?> UpdateAsync(Guid id, UpdateOSMarketResearchJuryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.UserIds = dto.UserIds;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<OSMarketResearchJuryDto>(existing);
        }
    }
}
