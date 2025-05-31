using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class ApproximateCostJuryService : IApproximateCostJuryService
    {
        private readonly MongoDBRepository<ApproximateCostJury> _repo;
        private readonly IMapper _mapper;

        public ApproximateCostJuryService(
            MongoDBRepository<ApproximateCostJury> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApproximateCostJuryDto> CreateAsync(CreateApproximateCostJuryDto dto)
        {
            try
            {
                var entity = _mapper.Map<ApproximateCostJury>(dto);
                entity.Id = Guid.NewGuid();
                await _repo.InsertAsync(entity);
                return _mapper.Map<ApproximateCostJuryDto>(entity);
            }
            catch (Exception ex)
            {
                // Log or console output
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<IEnumerable<ApproximateCostJuryDto>> GetAllByEntryAsync(Guid entryId)
        {
            var list = (await _repo.GetAllAsync())
                .Where(j => j.ProcurementEntryId == entryId);
            return list.Select(j => _mapper.Map<ApproximateCostJuryDto>(j));
        }

        public async Task<ApproximateCostJuryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<ApproximateCostJuryDto>(e);
        }

        public async Task<ApproximateCostJuryDto> UpdateAsync(Guid id, UpdateApproximateCostJuryDto dto)
        {
            var existing = (await _repo.GetAllAsync())
                .FirstOrDefault(j => j.Id == id);
            if (existing == null) return null;
            existing.UserIds = dto.UserIds;
            await _repo.UpdateAsync(existing.Id, existing);
            return _mapper.Map<ApproximateCostJuryDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Jüri kaydı bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
