using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class SubInspectionAcceptanceJuryService : ISubInspectionAcceptanceJuryService
    {
        private readonly MongoDBRepository<SubInspectionAcceptanceJury> _repo;
        private readonly IMapper _mapper;

        public SubInspectionAcceptanceJuryService(
            MongoDBRepository<SubInspectionAcceptanceJury> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<SubInspectionAcceptanceJuryDto> CreateAsync(CreateSubInspectionAcceptanceJuryDto dto)
        {
            var entity = _mapper.Map<SubInspectionAcceptanceJury>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<SubInspectionAcceptanceJuryDto>(entity);
        }

        public async Task<IEnumerable<SubInspectionAcceptanceJuryDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => _mapper.Map<SubInspectionAcceptanceJuryDto>(e));
        }

        public async Task<SubInspectionAcceptanceJuryDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<SubInspectionAcceptanceJuryDto>(e);
        }

        public async Task<SubInspectionAcceptanceJuryDto> UpdateAsync(Guid id, UpdateSubInspectionAcceptanceJuryDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;
            existing.UserIds = dto.UserIds;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<SubInspectionAcceptanceJuryDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) throw new KeyNotFoundException("Jüri kaydı bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
