using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class MarketResearchJuryService : IMarketResearchJuryService
    {
        private readonly MongoDBRepository<MarketResearchJury> _repo;
        private readonly IMapper _mapper;

        public MarketResearchJuryService(
            MongoDBRepository<MarketResearchJury> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<MarketResearchJuryDto> CreateAsync(CreateMarketResearchJuryDto dto)
        {
            try
            {
                var entity = _mapper.Map<MarketResearchJury>(dto);
                entity.Id = Guid.NewGuid();
                await _repo.InsertAsync(entity);
                return _mapper.Map<MarketResearchJuryDto>(entity);
            }
            catch (Exception ex)
            {
                // ex.Message ve ex.ToString()’u console’a veya log dosyanıza yazdırın
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<IEnumerable<MarketResearchJuryDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(j => j.ProcurementEntryId == entryId);
            return list.Select(j => _mapper.Map<MarketResearchJuryDto>(j));
        }

        public async Task<MarketResearchJuryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<MarketResearchJuryDto>(e);
        }

        public async Task<MarketResearchJuryDto> UpdateAsync(Guid id, UpdateMarketResearchJuryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.UserIds = dto.UserIds;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<MarketResearchJuryDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Jüri kaydı bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
