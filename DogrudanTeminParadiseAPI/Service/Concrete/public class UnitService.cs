using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class UnitService : IUnitService
    {
        private readonly MongoDBRepository<Unit> _repo;
        private readonly IMapper _mapper;

        public UnitService(MongoDBRepository<Unit> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UnitDto> CreateAsync(CreateUnitDto dto)
        {
            var all = await _repo.GetAllAsync();
            if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Bu isimde başka bir birim mevcut.");
            var entity = _mapper.Map<Unit>(dto);
            entity.Id = Guid.NewGuid();
            await _repo.InsertAsync(entity);
            return _mapper.Map<UnitDto>(entity);
        }

        public async Task<IEnumerable<UnitDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<UnitDto>(x));
        }

        public async Task<UnitDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<UnitDto>(e);
        }

        public async Task<UnitDto> UpdateAsync(Guid id, UpdateUnitDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;
            if (!existing.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var all = await _repo.GetAllAsync();
                if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Bu isimde başka bir birim mevcut.");
            }
            existing.Name = dto.Name;
            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<UnitDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen birim bulunamadı.");
            await _repo.DeleteAsync(id);
        }
    }
}
