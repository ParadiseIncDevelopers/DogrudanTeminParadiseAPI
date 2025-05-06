using AutoMapper;
using DogrudanTeminParadiseAPI.Dto;
using DogrudanTeminParadiseAPI.Models;
using DogrudanTeminParadiseAPI.Repositories;
using DogrudanTeminParadiseAPI.Service.Abstract;

namespace DogrudanTeminParadiseAPI.Service.Concrete
{
    public class AdministrationUnitService : IAdministrationUnitService
    {
        private readonly MongoDBRepository<AdministrationUnit> _repo;
        private readonly IMapper _mapper;

        public AdministrationUnitService(
            MongoDBRepository<AdministrationUnit> repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<AdministrationUnitDto> CreateAsync(CreateAdministrationUnitDto dto)
        {
            var all = await _repo.GetAllAsync();

            // Name duplicate kontrolü (case-insensitive)
            if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Bu isimde başka bir İdare Birimi zaten mevcut.");

            var entity = _mapper.Map<AdministrationUnit>(dto);
            entity.Id = Guid.NewGuid();

            await _repo.InsertAsync(entity);
            return _mapper.Map<AdministrationUnitDto>(entity);
        }

        public async Task<IEnumerable<AdministrationUnitDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => _mapper.Map<AdministrationUnitDto>(x));
        }

        public async Task<AdministrationUnitDto> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : _mapper.Map<AdministrationUnitDto>(e);
        }

        public async Task<AdministrationUnitDto> UpdateAsync(Guid id, UpdateAdministrationUnitDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            // Eğer isim değişmişse duplicate kontrolü
            if (!existing.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase))
            {
                var all = await _repo.GetAllAsync();
                if (all.Any(x => x.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Bu isimde başka bir İdare Birimi zaten mevcut.");
            }

            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.Description = dto.Description;

            await _repo.UpdateAsync(id, existing);
            return _mapper.Map<AdministrationUnitDto>(existing);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Silinmek istenen İdare Birimi bulunamadı.");

            await _repo.DeleteAsync(id);
        }
    }
}
